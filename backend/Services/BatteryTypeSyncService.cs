using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Services;

/// <summary>
/// Background service yang menyinkronkan data battery types
/// dari tabel legacy tlkp_item ke cos_battery_types setiap 30 menit.
/// </summary>
public class BatteryTypeSyncService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BatteryTypeSyncService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);

    public BatteryTypeSyncService(
        IServiceScopeFactory scopeFactory,
        ILogger<BatteryTypeSyncService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[BatterySyncService] Started. Interval: {Interval} minutes.", _interval.TotalMinutes);

        // Jalankan sync pertama kali saat startup
        await RunSyncAsync(stoppingToken);

        // Loop setiap 30 menit
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);
            await RunSyncAsync(stoppingToken);
        }
    }

    private async Task RunSyncAsync(CancellationToken ct)
    {
        _logger.LogWarning("══════════════════════════════════════════════════════");
        _logger.LogWarning("[BatterySyncService] Sync STARTED at {Time}", DateTime.Now);
        _logger.LogWarning("══════════════════════════════════════════════════════");

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FormCosDbContext>();

            // ── Step 1: Ambil semua data dari tabel legacy tlkp_item ──
            var legacyItems = await db.TlkpItems.ToListAsync(ct);

            if (legacyItems.Count == 0)
            {
                _logger.LogWarning("[BatterySyncService] ⚠ tabel tlkp_item KOSONG (0 rows). Tidak ada data untuk di-sync.");
                _logger.LogWarning("[BatterySyncService] Pastikan tabel tlkp_item di database '{Db}' sudah terisi data.",
                    "db_master_data");
                return;
            }

            _logger.LogWarning("[BatterySyncService] Ditemukan {Count} raw items di tlkp_item.", legacyItems.Count);

            // ── Step 2: Ambil semua battery types yang sudah ada (lookup cepat via HashSet) ──
            var existingTypes = await db.CosBatteryTypes.ToListAsync(ct);
            _logger.LogWarning("[BatterySyncService] Battery types yang sudah ada di cos_battery_types: {Count} records.", existingTypes.Count);

            var existingNames = new HashSet<string>(
                existingTypes.Select(t => t.Name.ToUpperInvariant()),
                StringComparer.OrdinalIgnoreCase
            );

            int inserted = 0;
            int skipped = 0;
            int filtered = 0;
            int errors = 0;

            // ── Step 3: Loop & Parse setiap item ──
            foreach (var item in legacyItems)
            {
                try
                {
                    // Skip jika item_num kosong
                    if (string.IsNullOrWhiteSpace(item.ItemNum))
                    {
                        skipped++;
                        continue;
                    }

                    var segments = item.ItemNum.Split('-');

                    // Minimal harus punya 3 segment: [0]=prefix, [1]=category, [2]=battery_name
                    if (segments.Length < 5)
                    {
                        skipped++;
                        continue;
                    }

                    // ── FILTER KETAT ──

                    // Filter Index 0: Harus "W" (case-insensitive)
                    if (!string.Equals(segments[0].Trim(), "W", StringComparison.OrdinalIgnoreCase))
                    {
                        filtered++;
                        continue;
                    }

                    // Filter Index 1: Harus mengandung huruf 'A' DAN huruf 'U'
                    var segment1Upper = segments[1].Trim().ToUpperInvariant();
                    
                    // Safety: Pastikan panjang minimal 3 karakter (karena kita mau cek huruf ke-3 / index [2])
                    if (segment1Upper.Length < 3)
                    {
                        filtered++;
                        continue;
                    }

                    // Rule: Huruf Pertama HARUS 'A' DAN Huruf Ketiga HARUS 'U'
                    // Contoh Valid: "AAUA", "AAUB"
                    // Contoh Invalid: "1AUA" (Salah awal), "AACA" (Salah urutan), "UCMA" (Salah awal)
                    if (segment1Upper[0] != 'A' || segment1Upper[2] != 'U')
                    {
                        filtered++;
                        continue;
                    }

                    // ── EXTRACT DATA ──
                    // var batteryName = segments[2].Trim();
                    var baseName = segments[2].Trim(); // Simpan base name sebelum ditambah suffix
                    var variant = segments[3].Trim();
                    var suffix = segments[4].Trim(); // Ambil suffix (misal: PRA0, PR0K)

                    // Format baru: "40B19L0 - MF00 (HPM)"
                    var batteryName = $"{baseName} - {suffix} ({variant})";

                    if (string.IsNullOrWhiteSpace(batteryName))
                    {
                        skipped++;
                        continue;
                    }

                    // ── UPSERT: Cek apakah sudah ada berdasarkan Name (case-insensitive) ──
                    if (existingNames.Contains(batteryName.ToUpperInvariant()))
                    {
                        skipped++;
                        continue;
                    }

                    // ── INSERT baru ──
                    var newType = new CosBatteryType
                    {
                        Name = batteryName,
                        SourceItemNum = item.ItemNum,
                        KatId = item.KatId
                    };

                    db.CosBatteryTypes.Add(newType);
                    existingNames.Add(batteryName.ToUpperInvariant()); // Cegah double insert dalam batch ini
                    inserted++;
                }
                catch (Exception ex)
                {
                    // Log error tapi JANGAN hentikan loop
                    errors++;
                    _logger.LogWarning(ex,
                        "[BatterySyncService] Error parsing item_num='{ItemNum}', kat_id={KatId}. Skipping.",
                        item.ItemNum, item.KatId);
                }
            }

            // ── Step 4: Save semua perubahan sekaligus (batch insert) ──
            if (inserted > 0)
            {
                await db.SaveChangesAsync(ct);
            }

            _logger.LogWarning("══════════════════════════════════════════════════════");
            _logger.LogWarning("[BatterySyncService] Sync COMPLETED.");
            _logger.LogWarning("  Total items di tlkp_item : {Total}", legacyItems.Count);
            _logger.LogWarning("  Lolos filter (W + A&U)   : {Passed}", legacyItems.Count - filtered);
            _logger.LogWarning("  Tidak lolos filter       : {Filtered}", filtered);
            _logger.LogWarning("  Baru di-INSERT           : {Inserted}", inserted);
            _logger.LogWarning("  Sudah ada (SKIP)         : {Skipped}", skipped);
            _logger.LogWarning("  Error parsing            : {Errors}", errors);
            _logger.LogWarning("══════════════════════════════════════════════════════");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BatterySyncService] Fatal error during sync cycle.");
        }
    }
}
