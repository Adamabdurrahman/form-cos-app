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

    // GET api/formdefinition
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var forms = await _db.FormDefinitions
            .Where(f => f.IsActive)
            .OrderBy(f => f.Code)
            .Select(f => new { f.Id, f.Code, f.Title, f.Subtitle, f.SlotCount, f.IsActive, f.CreatedAt })
            .ToListAsync();
        return Ok(forms);
    }

    // GET api/formdefinition/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var form = await _db.FormDefinitions
            .Include(f => f.CheckItems.OrderBy(c => c.SortOrder))
                .ThenInclude(c => c.SubRows.OrderBy(s => s.SortOrder))
            .Include(f => f.ProblemColumns.OrderBy(p => p.SortOrder))
            .Include(f => f.SignatureSlots.OrderBy(s => s.SortOrder))
            .FirstOrDefaultAsync(f => f.Id == id);

        if (form == null) return NotFound();
        return Ok(form);
    }

    // GET api/formdefinition/by-code/COS_VALIDATION
    [HttpGet("by-code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var form = await _db.FormDefinitions
            .Include(f => f.CheckItems.OrderBy(c => c.SortOrder))
                .ThenInclude(c => c.SubRows.OrderBy(s => s.SortOrder))
            .Include(f => f.ProblemColumns.OrderBy(p => p.SortOrder))
            .Include(f => f.SignatureSlots.OrderBy(s => s.SortOrder))
            .FirstOrDefaultAsync(f => f.Code == code);

        if (form == null) return NotFound();
        return Ok(form);
    }

    // POST api/formdefinition
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FormDefinition form)
    {
        form.CreatedAt = DateTime.UtcNow;
        _db.FormDefinitions.Add(form);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = form.Id }, form);
    }

    // PUT api/formdefinition/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] FormDefinition form)
    {
        var existing = await _db.FormDefinitions.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Code = form.Code;
        existing.Title = form.Title;
        existing.Subtitle = form.Subtitle;
        existing.SlotCount = form.SlotCount;
        existing.IsActive = form.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(existing);
    }

    // DELETE api/formdefinition/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.FormDefinitions.FindAsync(id);
        if (existing == null) return NotFound();
        _db.FormDefinitions.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
