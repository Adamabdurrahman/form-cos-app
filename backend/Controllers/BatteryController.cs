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

    // GET api/battery/types
    [HttpGet("types")]
    public async Task<IActionResult> GetTypes()
    {
        var types = await _db.CosBatteryTypes
            .Include(t => t.Standards)
            .OrderBy(t => t.Name)
            .ToListAsync();
        return Ok(types);
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
}
