using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckItemController : ControllerBase
{
    private readonly FormCosDbContext _db;
    public CheckItemController(FormCosDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetCheckItems()
    {
        var items = await _db.CosCheckItems
            .Include(ci => ci.SubRows.OrderBy(sr => sr.SortOrder))
            .OrderBy(ci => ci.SortOrder)
            .ToListAsync();

        var result = items.Select(ci => new
        {
            id = ci.ItemKey,
            ci.Label,
            ci.Type,
            ci.VisualStandard,
            ci.NumericStdKey,
            ci.FixedStandard,
            ci.Frequency,
            ci.Keterangan,
            ci.ConditionalLabel,
            SubRows = ci.SubRows.Any()
                ? ci.SubRows.Select(sr => new { sr.Suffix, sr.Label, sr.FixedStandard }).ToList()
                : null,
        });

        return Ok(result);
    }
}
