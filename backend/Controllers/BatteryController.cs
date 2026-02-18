using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BatteryController : ControllerBase
{
    private readonly MasterDbContext _masterDb;
    private readonly CosDbContext _cosDb;
    public BatteryController(MasterDbContext masterDb, CosDbContext cosDb)
    {
        _masterDb = masterDb;
        _cosDb = cosDb;
    }

    // GET api/battery/types
    [HttpGet("types")]
    public async Task<IActionResult> GetTypes()
    {
        var types = await _cosDb.CosBatteryTypes
            .Include(t => t.Standards)
            .OrderBy(t => t.Name)
            .ToListAsync();
        return Ok(types);
    }

    // GET api/battery/lines
    // Hanya tampilkan line dengan work_center LIKE 'WKAM%' (area COS)
    [HttpGet("lines")]
    public async Task<IActionResult> GetLines()
    {
        var lines = await _masterDb.TlkpLines
            .Where(l => l.LineStatus == 1
                     && l.WorkCenter != null
                     && l.WorkCenter.StartsWith("WKAM"))
            .OrderBy(l => l.LineId)
            .Select(l => new { id = l.LineId, name = l.LineName })
            .ToListAsync();
        return Ok(lines);
    }

    // GET api/battery/shifts
    [HttpGet("shifts")]
    public async Task<IActionResult> GetShifts()
    {
        var shifts = await _masterDb.TlkpShifts
            .Where(s => s.ShiftStatus == 1)
            .OrderBy(s => s.ShiftId)
            .ToListAsync();

        var result = shifts.Select(s => new
        {
            id = s.ShiftId,
            name = s.ShiftName,
            code = s.ShiftCode,
            start = s.ShiftStart.HasValue ? s.ShiftStart.Value.ToString(@"hh\:mm") : null,
            end = s.ShiftEnd.HasValue ? s.ShiftEnd.Value.ToString(@"hh\:mm") : null
        });
        return Ok(result);
    }

    // GET api/battery/molds
    [HttpGet("molds")]
    public async Task<IActionResult> GetMolds()
    {
        var molds = await _masterDb.TlkpMolds
            .Where(m => m.MoldStatus == "1")
            .OrderBy(m => m.MoldCode)
            .Select(m => new { code = m.MoldCode, description = m.MoldDescription })
            .ToListAsync();
        return Ok(molds);
    }

    /// <summary>
    /// GET api/battery/molds-by-battery?batteryName=W-GRID-BB8600S-00-0600
    /// Parses battery name → extracts keyword (index 2) → filters tlkp_mold by item_num.
    /// </summary>
    [HttpGet("molds-by-battery")]
    public async Task<IActionResult> GetMoldsByBattery([FromQuery] string? batteryName)
    {
        if (string.IsNullOrWhiteSpace(batteryName))
            return BadRequest(new { error = "batteryName is required" });

        // Parse: "W-GRID-BB8600S-00-0600" → split → index[2] = "BB8600S"
        var segments = batteryName.Split('-');
        if (segments.Length < 3)
        {
            // Format non-standard → return empty list (UI tetap jalan)
            return Ok(new { batteryName, keyword = (string?)null, count = 0, molds = Array.Empty<object>() });
        }

        var keyword = segments[2]; // e.g. "BB8600S"

        var molds = await _masterDb.TlkpMolds
            .Where(m => m.MoldStatus == "1" && m.ItemNum != null && m.ItemNum.Contains(keyword))
            .OrderBy(m => m.MoldCode)
            .Select(m => new { code = m.MoldCode, description = m.MoldDescription })
            .ToListAsync();

        return Ok(new { batteryName, keyword, count = molds.Count, molds });
    }
}
