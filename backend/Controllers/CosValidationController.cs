using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

/// <summary>
/// CRUD for COS validation form submissions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CosValidationController : ControllerBase
{
    private readonly FormCosDbContext _db;

    public CosValidationController(FormCosDbContext db) => _db = db;

    /// <summary>
    /// POST /api/cosvalidation — Submit a new COS validation form
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] CosValidationSubmitDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var validation = new CosValidation
        {
            Tanggal = dto.Tanggal,
            Line = dto.Line,
            Shift = dto.Shift,
            OperatorId = dto.OperatorId,
            LeaderId = dto.LeaderId,
            KasubsieId = dto.KasubsieId,
            KasieId = dto.KasieId,
            BatteryType1 = dto.BatteryType1,
            Mold1 = dto.Mold1,
            BatteryType2 = dto.BatteryType2,
            Mold2 = dto.Mold2,
            BatteryType3 = dto.BatteryType3,
            Mold3 = dto.Mold3,
            CreatedAt = DateTime.UtcNow,
        };

        // Add check settings
        foreach (var kvp in dto.Settings)
        {
            if (kvp.Value != null)
            {
                validation.CheckSettings.Add(new CosCheckSetting
                {
                    SettingKey = kvp.Key,
                    Value = kvp.Value,
                });
            }
        }

        // Add problems
        int order = 1;
        foreach (var prob in dto.Problems)
        {
            if (!string.IsNullOrWhiteSpace(prob.Problem) || !string.IsNullOrWhiteSpace(prob.Action))
            {
                validation.Problems.Add(new CosProblem
                {
                    Problem = prob.Problem,
                    Action = prob.Action,
                    SortOrder = order++,
                });
            }
        }

        // Add signatures
        foreach (var kvp in dto.Signatures)
        {
            if (!string.IsNullOrWhiteSpace(kvp.Value))
            {
                validation.Signatures.Add(new CosSignature
                {
                    Role = kvp.Key,
                    SignatureData = kvp.Value,
                });
            }
        }

        _db.CosValidations.Add(validation);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = validation.Id }, new { id = validation.Id });
    }

    /// <summary>
    /// GET /api/cosvalidation/{id} — Get a submitted form by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var validation = await _db.CosValidations
            .Include(v => v.Operator)
            .Include(v => v.Leader)
            .Include(v => v.Kasubsie)
            .Include(v => v.Kasie)
            .Include(v => v.CheckSettings)
            .Include(v => v.Problems.OrderBy(p => p.SortOrder))
            .Include(v => v.Signatures)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (validation == null)
            return NotFound(new { message = $"COS Validation {id} not found" });

        return Ok(new
        {
            validation.Id,
            validation.Tanggal,
            validation.Line,
            validation.Shift,
            @operator = validation.Operator != null ? new { validation.Operator.Id, validation.Operator.Name } : null,
            leader = validation.Leader != null ? new { validation.Leader.Id, validation.Leader.Name } : null,
            kasubsie = validation.Kasubsie != null ? new { validation.Kasubsie.Id, validation.Kasubsie.Name } : null,
            kasie = validation.Kasie != null ? new { validation.Kasie.Id, validation.Kasie.Name } : null,
            batteryType1 = validation.BatteryType1,
            mold1 = validation.Mold1,
            batteryType2 = validation.BatteryType2,
            mold2 = validation.Mold2,
            batteryType3 = validation.BatteryType3,
            mold3 = validation.Mold3,
            settings = validation.CheckSettings.ToDictionary(cs => cs.SettingKey, cs => cs.Value),
            problems = validation.Problems.Select(p => new { p.Problem, p.Action }),
            signatures = validation.Signatures.ToDictionary(s => s.Role, s => s.SignatureData),
            validation.CreatedAt,
        });
    }

    /// <summary>
    /// GET /api/cosvalidation — List all submitted forms (paginated)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = _db.CosValidations
            .Include(v => v.Operator)
            .OrderByDescending(v => v.CreatedAt);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new
            {
                v.Id,
                v.Tanggal,
                v.Line,
                v.Shift,
                OperatorName = v.Operator != null ? v.Operator.Name : null,
                v.CreatedAt,
            })
            .ToListAsync();

        return Ok(new
        {
            total,
            page,
            pageSize,
            items,
        });
    }

    /// <summary>
    /// DELETE /api/cosvalidation/{id} — Delete a submitted form
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var validation = await _db.CosValidations.FindAsync(id);
        if (validation == null)
            return NotFound(new { message = $"COS Validation {id} not found" });

        _db.CosValidations.Remove(validation);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
