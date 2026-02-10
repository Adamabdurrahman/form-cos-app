using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers;

/// <summary>
/// Returns check item definitions for the COS validation form.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CheckItemController : ControllerBase
{
    private readonly FormCosDbContext _db;

    public CheckItemController(FormCosDbContext db) => _db = db;

    /// <summary>
    /// GET /api/checkitem — List all check items with sub-rows
    /// Returns in the same shape as frontend's checkItems array:
    /// [{ id, label, type, visualStandard, numericStdKey, fixedStandard, frequency, keterangan, conditionalLabel, subRows }]
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCheckItems()
    {
        var items = await _db.CheckItems
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
                ? ci.SubRows.Select(sr => new
                {
                    sr.Suffix,
                    sr.Label,
                    sr.FixedStandard,
                }).ToList()
                : null,
        });

        return Ok(result);
    }
}
