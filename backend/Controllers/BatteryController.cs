using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BatteryController : ControllerBase
{
    private readonly FormCosDbContext _db;
    public BatteryController(FormCosDbContext db) => _db = db;

    // GET api/battery/types?lineId=2  (optional filter)
    [HttpGet("types")]
    public async Task<IActionResult> GetTypes([FromQuery] int? lineId)
    {
        if (lineId.HasValue)
        {
            // Return only battery types mapped to this line
            var typeIds = await _db.CosLineBatteryTypes
                .Where(lb => lb.LineId == lineId.Value)
                .Select(lb => lb.BatteryTypeId)
                .ToListAsync();

            var types = await _db.CosBatteryTypes
                .Include(t => t.Standards)
                .Where(t => typeIds.Contains(t.Id))
                .OrderBy(t => t.Name)
                .ToListAsync();
            return Ok(types);
        }

        // No filter — return all
        var all = await _db.CosBatteryTypes
            .Include(t => t.Standards)
            .OrderBy(t => t.Name)
            .ToListAsync();
        return Ok(all);
    }

    // GET api/battery/lines
    [HttpGet("lines")]
    public async Task<IActionResult> GetLines()
    {
        var lines = await _db.TlkpLines
            .Where(l => l.LineStatus == 1)
            .OrderBy(l => l.LineId)
            .Select(l => new { id = l.LineId, name = l.LineName })
            .ToListAsync();
        return Ok(lines);
    }

    // GET api/battery/shifts
    [HttpGet("shifts")]
    public async Task<IActionResult> GetShifts()
    {
        var shifts = await _db.TlkpShifts
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

    // GET api/battery/groups
    [HttpGet("groups")]
    public async Task<IActionResult> GetGroups()
    {
        var groups = await _db.TlkpGroups
            .Where(g => g.GroupStatus == 1)
            .OrderBy(g => g.GroupId)
            .Select(g => new { id = g.GroupId, name = g.GroupName })
            .ToListAsync();
        return Ok(groups);
    }

    // GET api/battery/molds
    [HttpGet("molds")]
    public async Task<IActionResult> GetMolds()
    {
        var molds = await _db.TlkpMolds
            .Where(m => m.MoldStatus == "1")
            .OrderBy(m => m.MoldCode)
            .Select(m => new { code = m.MoldCode, description = m.MoldDescription })
            .ToListAsync();
        return Ok(molds);
    }

    // GET api/battery/line-battery-types  (admin: full mapping)
    [HttpGet("line-battery-types")]
    public async Task<IActionResult> GetLineBatteryTypes()
    {
        var mapping = await _db.CosLineBatteryTypes
            .Include(lb => lb.BatteryType)
            .OrderBy(lb => lb.LineId).ThenBy(lb => lb.BatteryTypeId)
            .Select(lb => new { lb.Id, lb.LineId, lb.BatteryTypeId, batteryTypeName = lb.BatteryType!.Name })
            .ToListAsync();
        return Ok(mapping);
    }
}
