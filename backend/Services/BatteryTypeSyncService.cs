using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Services;

/// <summary>
/// Background service yang menyinkronkan data battery types
/// dari tabel legacy tlkp_item (MasterDbContext/db_master_data)
/// ke tlkp_cos_battery_types (CosDbContext/db_cos_checksheet) setiap 30 menit.
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
            var masterDb = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
            var cosDb = scope.ServiceProvider.GetRequiredService<CosDbContext>();

            // ── Step 1: Ambil semua data dari tabel legacy tlkp_item (db_master_data) ──
            var legacyItems = await masterDb.TlkpItems.ToListAsync(ct);

            if (legacyItems.Count == 0)
            {
                _logger.LogWarning("[BatterySyncService] ⚠ tabel tlkp_item KOSONG (0 rows). Tidak ada data untuk di-sync.");
                _logger.LogWarning("[BatterySyncService] Pastikan tabel tlkp_item di database '{Db}' sudah terisi data.",
                    "db_master_data");
                return;
            }

            _logger.LogWarning("[BatterySyncService] Ditemukan {Count} raw items di tlkp_item.", legacyItems.Count);

            // ── Step 2: Ambil semua battery types yang sudah ada (db_cos_checksheet) ──
            var existingTypes = await cosDb.CosBatteryTypes.ToListAsync(ct);
            _logger.LogWarning("[BatterySyncService] Battery types yang sudah ada di tlkp_cos_battery_types: {Count} records.", existingTypes.Count);

            var existingNames = new HashSet<string>(
                existingTypes.Select(t => t.Name.ToUpperInvariant()),
                StringComparer.OrdinalIgnoreCase
            );

            int inserted = 0;
            int skipped = 0;
            int errors = 0;

            // ── Step 3: Loop & sync setiap item ──
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

                    /* ------------------- OLD PARSING LOGIC (ARCHIVED) -------------------
                    var segments = item.ItemNum.Split('-');

                    // Minimal harus punya 5 segment
                    if (segments.Length < 5)
                    {
                        skipped++;
                        continue;
                    }

                    // Filter Index 0: Harus "W" (case-insensitive)
                    if (!string.Equals(segments[0].Trim(), "W", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // Filter Index 1: Huruf Pertama HARUS 'A' DAN Huruf Ketiga HARUS 'U'
                    var segment1Upper = segments[1].Trim().ToUpperInvariant();
                    if (segment1Upper.Length < 3)
                    {
                        continue;
                    }
                    if (segment1Upper[0] != 'A' || segment1Upper[2] != 'U')
                    {
                        continue;
                    }

                    // EXTRACT DATA
                    var baseName = segments[2].Trim();
                    var variant = segments[3].Trim();
                    var suffix = segments[4].Trim();

                    // Format: "40B19L0 - MF00 (HPM)"
                    var batteryName = $"{baseName} - {suffix} ({variant})";

                    if (string.IsNullOrWhiteSpace(batteryName))
                    {
                        skipped++;
                        continue;
                    }

                    if (existingNames.Contains(batteryName.ToUpperInvariant()))
                    {
                        skipped++;
                        continue;
                    }

                    var newType = new CosBatteryType
                    {
                        Name = batteryName,
                        SourceItemNum = item.ItemNum,
                        KatId = item.KatId,
                        FormId = 1
                    };
                    cosDb.CosBatteryTypes.Add(newType);
                    existingNames.Add(batteryName.ToUpperInvariant());
                    inserted++;
                    ------------------- END OLD PARSING LOGIC ------------------- */

                    // ------------------- NEW SIMPLE LOGIC -------------------
                    // Langsung pakai item.ItemNum mentah tanpa parsing/filter apapun.

                    // Duplicate check: skip jika sudah ada di DB
                    if (existingNames.Contains(item.ItemNum.ToUpperInvariant()))
                    {
                        skipped++;
                        continue;
                    }

                    // INSERT baru ke db_cos_checksheet
                    var newType = new CosBatteryType
                    {
                        Name = item.ItemNum,           // Persis apa adanya
                        SourceItemNum = item.ItemNum,
                        KatId = item.KatId,
                        FormId = 1                     // Default: COS_VALIDATION form
                    };

                    cosDb.CosBatteryTypes.Add(newType);
                    existingNames.Add(item.ItemNum.ToUpperInvariant()); // Cegah double insert dalam batch ini
                    inserted++;
                }
                catch (Exception ex)
                {
                    // Log error tapi JANGAN hentikan loop
                    errors++;
                    _logger.LogWarning(ex,
                        "[BatterySyncService] Error processing item_num='{ItemNum}', kat_id={KatId}. Skipping.",
                        item.ItemNum, item.KatId);
                }
            }

            // ── Step 4: Save semua perubahan sekaligus (batch insert ke db_cos_checksheet) ──
            if (inserted > 0)
            {
                await cosDb.SaveChangesAsync(ct);
            }

            _logger.LogWarning("══════════════════════════════════════════════════════");
            _logger.LogWarning("[BatterySyncService] Sync COMPLETED.");
            _logger.LogWarning("  Total items di tlkp_item : {Total}", legacyItems.Count);
            _logger.LogWarning("  Baru di-INSERT           : {Inserted}", inserted);
            _logger.LogWarning("  Sudah ada (SKIP)         : {Skipped}", skipped);
            _logger.LogWarning("  Error processing         : {Errors}", errors);
            _logger.LogWarning("══════════════════════════════════════════════════════");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BatterySyncService] Fatal error during sync cycle.");
        }
    }
}
