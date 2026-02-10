using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormDefinitionController : ControllerBase
{
    private readonly FormCosDbContext _db;
    public FormDefinitionController(FormCosDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var defs = await _db.CosFormDefinitions
            .OrderBy(d => d.Id)
            .Select(d => new { d.Id, d.Code, d.Title, d.Subtitle, d.SlotCount, d.IsActive })
            .ToListAsync();
        return Ok(defs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var def = await _db.CosFormDefinitions
            .Include(d => d.CheckItems).ThenInclude(c => c.SubRows)
            .Include(d => d.ProblemColumns)
            .Include(d => d.SignatureSlots)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (def == null) return NotFound();

        def.CheckItems = def.CheckItems.OrderBy(c => c.SortOrder).ToList();
        foreach (var ci in def.CheckItems)
            ci.SubRows = ci.SubRows.OrderBy(s => s.SortOrder).ToList();
        def.ProblemColumns = def.ProblemColumns.OrderBy(p => p.SortOrder).ToList();
        def.SignatureSlots = def.SignatureSlots.OrderBy(s => s.SortOrder).ToList();
        return Ok(def);
    }

    [HttpGet("by-code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var def = await _db.CosFormDefinitions
            .Include(d => d.CheckItems).ThenInclude(c => c.SubRows)
            .Include(d => d.ProblemColumns)
            .Include(d => d.SignatureSlots)
            .FirstOrDefaultAsync(d => d.Code == code);
        if (def == null) return NotFound();

        def.CheckItems = def.CheckItems.OrderBy(c => c.SortOrder).ToList();
        foreach (var ci in def.CheckItems)
            ci.SubRows = ci.SubRows.OrderBy(s => s.SortOrder).ToList();
        def.ProblemColumns = def.ProblemColumns.OrderBy(p => p.SortOrder).ToList();
        def.SignatureSlots = def.SignatureSlots.OrderBy(s => s.SortOrder).ToList();
        return Ok(def);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CosFormDefinition def)
    {
        def.CreatedAt = DateTime.UtcNow;
        _db.CosFormDefinitions.Add(def);
        await _db.SaveChangesAsync();
        return Ok(def);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] System.Text.Json.JsonElement body)
    {
        var existing = await _db.CosFormDefinitions.FindAsync(id);
        if (existing == null) return NotFound();

        if (body.TryGetProperty("code", out var c)) existing.Code = c.GetString()!;
        if (body.TryGetProperty("title", out var t)) existing.Title = t.GetString()!;
        if (body.TryGetProperty("subtitle", out var s)) existing.Subtitle = s.GetString();
        if (body.TryGetProperty("slotCount", out var sc)) existing.SlotCount = sc.GetInt32();
        if (body.TryGetProperty("isActive", out var ia)) existing.IsActive = ia.GetBoolean();
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.CosFormDefinitions.FindAsync(id);
        if (existing == null) return NotFound();
        _db.CosFormDefinitions.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
