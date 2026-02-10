using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers;

/// <summary>
/// Returns battery type reference data including molds and standards.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BatteryController : ControllerBase
{
    private readonly FormCosDbContext _db;

    public BatteryController(FormCosDbContext db) => _db = db;

    /// <summary>
    /// GET /api/battery/types — List all battery types with molds and standards
    /// Returns in the same shape as frontend's batteryTypes array:
    /// [{ name, molds: string[], standards: { key: value } }]
    /// </summary>
    [HttpGet("types")]
    public async Task<IActionResult> GetBatteryTypes()
    {
        var types = await _db.BatteryTypes
            .Include(bt => bt.Molds)
            .Include(bt => bt.Standards)
            .OrderBy(bt => bt.Id)
            .ToListAsync();

        var result = types.Select(bt => new
        {
            bt.Name,
            Molds = bt.Molds.OrderBy(m => m.Id).Select(m => m.Name).ToList(),
            Standards = bt.Standards.ToDictionary(s => s.ParamKey, s => s.Value)
        });

        return Ok(result);
    }

    /// <summary>
    /// GET /api/battery/lines — Line numbers available for COS
    /// </summary>
    [HttpGet("lines")]
    public IActionResult GetLines()
    {
        return Ok(new[] { 2, 3, 4, 5, 6, 7, 8 });
    }

    /// <summary>
    /// GET /api/battery/shifts — Shift numbers available
    /// </summary>
    [HttpGet("shifts")]
    public IActionResult GetShifts()
    {
        return Ok(new[] { 1, 2, 3 });
    }
}
