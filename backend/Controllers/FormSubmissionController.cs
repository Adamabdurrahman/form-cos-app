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
                s.Status, s.HasNg,
                s.OperatorSignedAt, s.LeaderSignedAt, s.KasubsieSignedAt, s.KasieSignedAt,
                FormCode = s.Form != null ? s.Form.Code : null,
                FormTitle = s.Form != null ? s.Form.Title : null,
            })
            .ToListAsync();

        var empIds = submissions
            .SelectMany(s => new[] { s.OperatorEmpId, s.LeaderEmpId, s.KasubsieEmpId, s.KasieEmpId })
            .Where(e => e != null).Distinct().ToList();
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
            s.Status, s.HasNg,
            s.OperatorSignedAt, s.LeaderSignedAt, s.KasubsieSignedAt, s.KasieSignedAt,
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
            sub.LineId, sub.ShiftId, sub.GroupId,
            sub.OperatorEmpId, sub.LeaderEmpId, sub.KasubsieEmpId, sub.KasieEmpId,
            OperatorName = ResolveName(sub.OperatorEmpId),
            LeaderName = ResolveName(sub.LeaderEmpId),
            KasubsieName = ResolveName(sub.KasubsieEmpId),
            KasieName = ResolveName(sub.KasieEmpId),
            sub.BatterySlotsJson, sub.CreatedAt,
            // Approval workflow
            sub.Status, sub.HasNg,
            sub.OperatorSignedAt, sub.LeaderSignedAt, sub.KasubsieSignedAt, sub.KasieSignedAt,
            sub.LeaderMemo, sub.KasubsieMemo, sub.KasieMemo,
            Form = sub.Form != null ? new { sub.Form.Id, sub.Form.Code, sub.Form.Title, sub.Form.Subtitle } : null,
            CheckValues = sub.CheckValues.Select(cv => new { cv.Id, cv.SettingKey, cv.Value }),
            Problems = sub.Problems.OrderBy(p => p.SortOrder).Select(p => new { p.Id, p.SortOrder, p.ValuesJson }),
            Signatures = sub.Signatures.Select(sig => new { sig.Id, sig.RoleKey, sig.SignatureData, sig.SignedAt }),
        });
    }

    // POST api/formsubmission
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CosSubmission submission)
    {
        submission.CreatedAt = DateTime.UtcNow;
        submission.Status = "pending_leader";
        submission.OperatorSignedAt = DateTime.UtcNow;

        // Detect NG: check if any check value is 'ng'
        if (submission.CheckValues != null)
        {
            submission.HasNg = submission.CheckValues.Any(cv =>
                cv.Value != null && cv.Value.Equals("ng", StringComparison.OrdinalIgnoreCase));
            foreach (var cv in submission.CheckValues) cv.SubmissionId = 0;
        }
        if (submission.Problems != null)
            foreach (var p in submission.Problems) p.SubmissionId = 0;
        if (submission.Signatures != null)
        {
            foreach (var s in submission.Signatures)
            {
                s.SubmissionId = 0;
                // Set signed_at for operator signature
                if (s.RoleKey == "operator" && s.SignatureData != null)
                    s.SignedAt = DateTime.UtcNow;
            }
        }

        _db.CosSubmissions.Add(submission);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = submission.Id }, new { id = submission.Id, hasNg = submission.HasNg });
    }

    // PUT api/formsubmission/{id}/approve
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> Approve(int id, [FromBody] ApproveDto dto)
    {
        var sub = await _db.CosSubmissions
            .Include(s => s.Signatures)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (sub == null) return NotFound();

        var role = dto.Role?.ToLower();
        var now = DateTime.UtcNow;

        // Validate role matches current status
        var expectedRole = sub.Status switch
        {
            "pending_leader" => "leader",
            "pending_kasubsie" => "kasubsie",
            "pending_kasie" => "kasie",
            _ => null,
        };

        if (expectedRole == null)
            return BadRequest(new { error = "Submission sudah completed atau status tidak valid." });
        if (role != expectedRole)
            return BadRequest(new { error = $"Giliran {expectedRole} untuk approve, bukan {role}." });

        // If hasNg, memo is required
        if (sub.HasNg && string.IsNullOrWhiteSpace(dto.Memo))
            return BadRequest(new { error = "Memo wajib diisi karena terdapat item NG." });

        // Update signature entry
        var sigEntry = sub.Signatures.FirstOrDefault(s => s.RoleKey == role);
        if (sigEntry != null)
        {
            sigEntry.SignatureData = dto.SignatureData;
            sigEntry.SignedAt = now;
        }
        else
        {
            _db.CosSignatureEntries.Add(new CosSignatureEntry
            {
                SubmissionId = id,
                RoleKey = role,
                SignatureData = dto.SignatureData,
                SignedAt = now,
            });
        }

        // Set memo + timestamp based on role
        switch (role)
        {
            case "leader":
                sub.LeaderMemo = dto.Memo;
                sub.LeaderSignedAt = now;
                sub.Status = "pending_kasubsie";
                break;
            case "kasubsie":
                sub.KasubsieMemo = dto.Memo;
                sub.KasubsieSignedAt = now;
                sub.Status = "pending_kasie";
                break;
            case "kasie":
                sub.KasieMemo = dto.Memo;
                sub.KasieSignedAt = now;
                sub.Status = "completed";
                break;
        }

        sub.UpdatedAt = now;
        await _db.SaveChangesAsync();

        return Ok(new { id = sub.Id, status = sub.Status });
    }

    // PUT api/formsubmission/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CosSubmission input)
    {
        var existing = await _db.CosSubmissions
            .Include(s => s.CheckValues)
            .Include(s => s.Problems)
            .Include(s => s.Signatures)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (existing == null) return NotFound();

        existing.FormId = input.FormId;
        existing.Tanggal = input.Tanggal;
        existing.LineId = input.LineId;
        existing.ShiftId = input.ShiftId;
        existing.GroupId = input.GroupId;
        existing.OperatorEmpId = input.OperatorEmpId;
        existing.LeaderEmpId = input.LeaderEmpId;
        existing.KasubsieEmpId = input.KasubsieEmpId;
        existing.KasieEmpId = input.KasieEmpId;
        existing.BatterySlotsJson = input.BatterySlotsJson;
        existing.UpdatedAt = DateTime.UtcNow;

        // Re-detect NG
        _db.CosCheckValues.RemoveRange(existing.CheckValues);
        if (input.CheckValues != null)
        {
            existing.HasNg = input.CheckValues.Any(cv =>
                cv.Value != null && cv.Value.Equals("ng", StringComparison.OrdinalIgnoreCase));
            foreach (var cv in input.CheckValues)
            {
                cv.Id = 0;
                cv.SubmissionId = existing.Id;
                _db.CosCheckValues.Add(cv);
            }
        }

        _db.CosProblems.RemoveRange(existing.Problems);
        if (input.Problems != null)
        {
            foreach (var p in input.Problems)
            {
                p.Id = 0;
                p.SubmissionId = existing.Id;
                _db.CosProblems.Add(p);
            }
        }

        _db.CosSignatureEntries.RemoveRange(existing.Signatures);
        if (input.Signatures != null)
        {
            foreach (var s in input.Signatures)
            {
                s.Id = 0;
                s.SubmissionId = existing.Id;
                _db.CosSignatureEntries.Add(s);
            }
        }

        await _db.SaveChangesAsync();
        return Ok(new { id = existing.Id });
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

// DTO for approval endpoint
public class ApproveDto
{
    public string? Role { get; set; }
    public string? SignatureData { get; set; }
    public string? Memo { get; set; }
}
