using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers;

/// <summary>
/// Returns personnel hierarchy data (Kasie → Kasubsie → Leader → Operator).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PersonnelController : ControllerBase
{
    private readonly FormCosDbContext _db;

    public PersonnelController(FormCosDbContext db) => _db = db;

    /// <summary>
    /// GET /api/personnel/operators — List all operators
    /// </summary>
    [HttpGet("operators")]
    public async Task<IActionResult> GetOperators()
    {
        var data = await _db.Operators
            .OrderBy(o => o.Name)
            .Select(o => new { o.Id, o.Name, o.LeaderId })
            .ToListAsync();
        return Ok(data);
    }

    /// <summary>
    /// GET /api/personnel/leaders — List all leaders
    /// </summary>
    [HttpGet("leaders")]
    public async Task<IActionResult> GetLeaders()
    {
        var data = await _db.Leaders
            .OrderBy(l => l.Name)
            .Select(l => new { l.Id, l.Name, l.KasubsieId })
            .ToListAsync();
        return Ok(data);
    }

    /// <summary>
    /// GET /api/personnel/kasubsies — List all kasubsie
    /// </summary>
    [HttpGet("kasubsies")]
    public async Task<IActionResult> GetKasubsies()
    {
        var data = await _db.Kasubsies
            .OrderBy(k => k.Name)
            .Select(k => new { k.Id, k.Name, k.KasieId })
            .ToListAsync();
        return Ok(data);
    }

    /// <summary>
    /// GET /api/personnel/kasies — List all kasie
    /// </summary>
    [HttpGet("kasies")]
    public async Task<IActionResult> GetKasies()
    {
        var data = await _db.Kasies
            .OrderBy(k => k.Name)
            .Select(k => new { k.Id, k.Name })
            .ToListAsync();
        return Ok(data);
    }

    /// <summary>
    /// GET /api/personnel/hierarchy/{operatorId} — Resolve full hierarchy for an operator
    /// Returns: { operator, leader, kasubsie, kasie }
    /// </summary>
    [HttpGet("hierarchy/{operatorId:int}")]
    public async Task<IActionResult> GetHierarchy(int operatorId)
    {
        var op = await _db.Operators
            .Include(o => o.Leader)
                .ThenInclude(l => l!.Kasubsie)
                    .ThenInclude(k => k!.Kasie)
            .FirstOrDefaultAsync(o => o.Id == operatorId);

        if (op == null) return NotFound(new { message = $"Operator {operatorId} not found" });

        return Ok(new
        {
            @operator = new { op.Id, op.Name },
            leader = op.Leader != null ? new { op.Leader.Id, op.Leader.Name } : null,
            kasubsie = op.Leader?.Kasubsie != null ? new { op.Leader.Kasubsie.Id, op.Leader.Kasubsie.Name } : null,
            kasie = op.Leader?.Kasubsie?.Kasie != null ? new { op.Leader.Kasubsie.Kasie.Id, op.Leader.Kasubsie.Kasie.Name } : null,
        });
    }
}
