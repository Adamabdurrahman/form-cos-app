using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Services;

/// <summary>
/// Reads real-time production data from db_padCost
/// and maps it to COS battery types.
/// </summary>
public class ProductionDataService
{
    private readonly PadCostDbContext _padCostDb;
    private readonly CosDbContext _cosDb;
    private readonly ILogger<ProductionDataService> _logger;

    public ProductionDataService(
        PadCostDbContext padCostDb,
        CosDbContext cosDb,
        ILogger<ProductionDataService> logger)
    {
        _padCostDb = padCostDb;
        _cosDb = cosDb;
        _logger = logger;
    }

    /// <summary>
    /// Get battery types currently running on a given line/shift/date.
    /// </summary>
    public async Task<List<RunningBatteryDto>> GetRunningBatteriesAsync(int lineId, int shiftId, DateTime? date = null)
    {
        var targetDate = date ?? DateTime.Today;

        _logger.LogInformation(
            "[ProductionData] GetRunningBatteries: lineId={LineId}, shiftId={ShiftId}, date={Date}",
            lineId, shiftId, targetDate.ToString("yyyy-MM-dd"));

        // ── Step 1: Query db_padCost for DISTINCT actl_item ──
        var runningItems = await _padCostDb.Achievements
            .Join(
                _padCostDb.ActAchievements,
                a => a.AchiId,
                b => b.AchiId,
                (a, b) => new { Achievement = a, ActAchievement = b }
            )
            .Where(x => x.Achievement.AchiDate.HasValue
                      && x.Achievement.AchiDate.Value.Date == targetDate.Date
                      && x.Achievement.ShiftId == shiftId
                      && x.ActAchievement.LineId == lineId
                      && x.ActAchievement.ActlItem != null
                      && x.ActAchievement.ActlItem != "")
            .Select(x => x.ActAchievement.ActlItem!)
            .Distinct()
            .ToListAsync();

        _logger.LogInformation(
            "[ProductionData] PadCost returned {Count} distinct item(s): [{Items}]",
            runningItems.Count, string.Join(", ", runningItems));

        if (runningItems.Count == 0)
            return new List<RunningBatteryDto>();

        // ── Step 2: Match to CosBatteryTypes via SourceItemNum or Name ──
        var matchedBatteries = await _cosDb.CosBatteryTypes
            .Where(bt => runningItems.Contains(bt.SourceItemNum!)
                      || runningItems.Contains(bt.Name))
            .Select(bt => new RunningBatteryDto
            {
                Id = bt.Id,
                Name = bt.Name,
                SourceItemNum = bt.SourceItemNum,
                FormId = bt.FormId
            })
            .ToListAsync();

        // Deduplicate by Name (case-insensitive)
        matchedBatteries = matchedBatteries
            .GroupBy(b => b.Name.ToUpperInvariant())
            .Select(g => g.First())
            .ToList();

        _logger.LogInformation(
            "[ProductionData] Matched {Count} COS battery type(s)", matchedBatteries.Count);

        return matchedBatteries;
    }
}

/// <summary>
/// DTO returned by the running-batteries endpoint.
/// </summary>
public class RunningBatteryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? SourceItemNum { get; set; }
    public int? FormId { get; set; }
}
