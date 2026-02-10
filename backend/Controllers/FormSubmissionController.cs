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
        var query = _db.CosSubmissions.AsQueryable();
        if (formId.HasValue) query = query.Where(s => s.FormId == formId.Value);

        var submissions = await query
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new
            {
                s.Id, s.FormId, s.Tanggal,
                s.LineId, s.ShiftId,
                s.OperatorEmpId, s.LeaderEmpId, s.KasubsieEmpId, s.KasieEmpId,
                s.BatterySlotsJson, s.CreatedAt,
                FormCode = s.Form != null ? s.Form.Code : null,
                FormTitle = s.Form != null ? s.Form.Title : null,
            })
            .ToListAsync();

        // Resolve operator names from ViewDataAuth
        var empIds = submissions.Select(s => s.OperatorEmpId).Where(e => e != null).Distinct().ToList();
        var nameMap = await _db.ViewDataAuths
            .Where(a => empIds.Contains(a.EmpId))
            .ToDictionaryAsync(a => a.EmpId!, a => a.FullName ?? a.EmpId ?? "");

        var result = submissions.Select(s => new
        {
            s.Id, s.FormId, s.Tanggal,
            s.LineId, s.ShiftId,
            s.OperatorEmpId, s.LeaderEmpId, s.KasubsieEmpId, s.KasieEmpId,
            OperatorName = s.OperatorEmpId != null && nameMap.TryGetValue(s.OperatorEmpId, out var n) ? n : s.OperatorEmpId,
            s.BatterySlotsJson, s.CreatedAt,
            s.FormCode, s.FormTitle,
        });

        return Ok(result);
    }

    // GET api/formsubmission/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sub = await _db.CosSubmissions
            .Include(s => s.CheckValues)
            .Include(s => s.Problems)
            .Include(s => s.Signatures)
            .Include(s => s.Form)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sub == null) return NotFound();

        // Resolve all personnel names
        var empIds = new[] { sub.OperatorEmpId, sub.LeaderEmpId, sub.KasubsieEmpId, sub.KasieEmpId }
            .Where(e => e != null).Cast<string>().Distinct().ToList();
        var nameMap = await _db.ViewDataAuths
            .Where(a => empIds.Contains(a.EmpId))
            .ToDictionaryAsync(a => a.EmpId!, a => a.FullName ?? a.EmpId ?? "");

        string ResolveName(string? empId) =>
            empId != null && nameMap.TryGetValue(empId, out var n) ? n : empId ?? "";

        return Ok(new
        {
            sub.Id, sub.FormId, sub.Tanggal,
            sub.LineId, sub.ShiftId,
            sub.OperatorEmpId, sub.LeaderEmpId, sub.KasubsieEmpId, sub.KasieEmpId,
            OperatorName = ResolveName(sub.OperatorEmpId),
            LeaderName = ResolveName(sub.LeaderEmpId),
            KasubsieName = ResolveName(sub.KasubsieEmpId),
            KasieName = ResolveName(sub.KasieEmpId),
            sub.BatterySlotsJson, sub.CreatedAt,
            Form = sub.Form != null ? new { sub.Form.Id, sub.Form.Code, sub.Form.Title, sub.Form.Subtitle } : null,
            CheckValues = sub.CheckValues.Select(cv => new { cv.Id, cv.SettingKey, cv.Value }),
            Problems = sub.Problems.OrderBy(p => p.SortOrder).Select(p => new { p.Id, p.SortOrder, p.ValuesJson }),
            Signatures = sub.Signatures.Select(sig => new { sig.Id, sig.RoleKey, sig.SignatureData }),
        });
    }

    // POST api/formsubmission
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CosSubmission submission)
    {
        submission.CreatedAt = DateTime.UtcNow;

        if (submission.CheckValues != null)
            foreach (var cv in submission.CheckValues) cv.SubmissionId = 0;
        if (submission.Problems != null)
            foreach (var p in submission.Problems) p.SubmissionId = 0;
        if (submission.Signatures != null)
            foreach (var s in submission.Signatures) s.SubmissionId = 0;

        _db.CosSubmissions.Add(submission);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = submission.Id }, new { id = submission.Id });
    }

    // DELETE api/formsubmission/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sub = await _db.CosSubmissions.FindAsync(id);
        if (sub == null) return NotFound();
        _db.CosSubmissions.Remove(sub);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
