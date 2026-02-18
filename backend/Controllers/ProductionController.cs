using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionController : ControllerBase
{
    private readonly ProductionDataService _productionService;

    public ProductionController(ProductionDataService productionService)
    {
        _productionService = productionService;
    }

    /// <summary>
    /// GET /api/production/running-batteries?lineId=3&shiftId=1&date=2026-02-17
    /// Returns battery types currently being produced on the given line/shift/date.
    /// If date is omitted, defaults to today.
    /// </summary>
    [HttpGet("running-batteries")]
    public async Task<IActionResult> GetRunningBatteries(
        [FromQuery] int lineId,
        [FromQuery] int shiftId,
        [FromQuery] DateTime? date = null)
    {
        if (lineId <= 0 || shiftId <= 0)
            return BadRequest(new { error = "lineId and shiftId are required and must be > 0" });

        var targetDate = date ?? DateTime.Today;
        var batteries = await _productionService.GetRunningBatteriesAsync(lineId, shiftId, targetDate);

        return Ok(new
        {
            date = targetDate.ToString("yyyy-MM-dd"),
            lineId,
            shiftId,
            count = batteries.Count,
            batteries
        });
    }
}
