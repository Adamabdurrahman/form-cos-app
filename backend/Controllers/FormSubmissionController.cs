using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormSubmissionController : ControllerBase
{
    private readonly FormCosDbContext _db;
    public FormSubmissionController(FormCosDbContext db) => _db = db;

    // GET api/formsubmission?formId=1
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? formId)
    {
        var query = _db.FormSubmissions.AsQueryable();
        if (formId.HasValue) query = query.Where(s => s.FormId == formId.Value);

        var submissions = await query
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new
            {
                s.Id, s.FormId, s.Tanggal, s.Line, s.Shift,
                s.OperatorId, s.LeaderId, s.KasubsieId, s.KasieId,
                s.BatterySlotsJson, s.CreatedAt,
                OperatorName = s.Operator != null ? s.Operator.Name : null,
                FormCode = s.Form != null ? s.Form.Code : null,
                FormTitle = s.Form != null ? s.Form.Title : null,
            })
            .ToListAsync();
        return Ok(submissions);
    }

    // GET api/formsubmission/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sub = await _db.FormSubmissions
            .Where(s => s.Id == id)
            .Select(s => new
            {
                s.Id, s.FormId, s.Tanggal, s.Line, s.Shift,
                s.OperatorId, s.LeaderId, s.KasubsieId, s.KasieId,
                s.BatterySlotsJson, s.CreatedAt,
                Operator = s.Operator != null ? new { s.Operator.Id, s.Operator.Name } : null,
                Leader = s.Leader != null ? new { s.Leader.Id, s.Leader.Name } : null,
                Kasubsie = s.Kasubsie != null ? new { s.Kasubsie.Id, s.Kasubsie.Name } : null,
                Kasie = s.Kasie != null ? new { s.Kasie.Id, s.Kasie.Name } : null,
                Form = s.Form != null ? new { s.Form.Id, s.Form.Code, s.Form.Title, s.Form.Subtitle } : null,
                CheckValues = s.CheckValues.Select(cv => new { cv.Id, cv.SettingKey, cv.Value }).ToList(),
                Problems = s.Problems.OrderBy(p => p.SortOrder).Select(p => new { p.Id, p.SortOrder, p.ValuesJson }).ToList(),
                Signatures = s.Signatures.Select(sig => new { sig.Id, sig.RoleKey, sig.SignatureData }).ToList(),
            })
            .FirstOrDefaultAsync();

        if (sub == null) return NotFound();
        return Ok(sub);
    }

    // POST api/formsubmission
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FormSubmission submission)
    {
        submission.CreatedAt = DateTime.UtcNow;

        // Attach child entities
        if (submission.CheckValues != null)
            foreach (var cv in submission.CheckValues) cv.SubmissionId = 0; // EF will handle

        if (submission.Problems != null)
            foreach (var p in submission.Problems) p.SubmissionId = 0;

        if (submission.Signatures != null)
            foreach (var s in submission.Signatures) s.SubmissionId = 0;

        _db.FormSubmissions.Add(submission);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = submission.Id }, new { id = submission.Id });
    }

    // DELETE api/formsubmission/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sub = await _db.FormSubmissions.FindAsync(id);
        if (sub == null) return NotFound();
        _db.FormSubmissions.Remove(sub);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
