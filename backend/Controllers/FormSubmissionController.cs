using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormSubmissionController : ControllerBase
{
    private readonly CosDbContext _cosDb;
    private readonly MasterDbContext _masterDb;
    public FormSubmissionController(CosDbContext cosDb, MasterDbContext masterDb)
    {
        _cosDb = cosDb;
        _masterDb = masterDb;
    }

    // GET api/formsubmission?formId=1
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? formId)
    {
        var query = _cosDb.CosSubmissions.AsQueryable();
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

        // Resolve operator names from ViewDataAuth (cross-database → MasterDbContext)
        var empIds = submissions.Select(s => s.OperatorEmpId).Where(e => e != null).Distinct().ToList();
        var nameMap = await _masterDb.ViewDataAuths
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
            s.Status, s.HasNg,
            s.OperatorSignedAt, s.LeaderSignedAt, s.KasubsieSignedAt, s.KasieSignedAt,
        });

        return Ok(result);
    }

    // GET api/formsubmission/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sub = await _cosDb.CosSubmissions
            .Include(s => s.CheckValues)
            .Include(s => s.Problems)
            .Include(s => s.Signatures)
            .Include(s => s.Form)
            .Include(s => s.ApprovalAttachments)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sub == null) return NotFound();

        // Resolve all personnel names (cross-database → MasterDbContext)
        var empIds = new[] { sub.OperatorEmpId, sub.LeaderEmpId, sub.KasubsieEmpId, sub.KasieEmpId }
            .Where(e => e != null).Cast<string>().Distinct().ToList();
        var nameMap = await _masterDb.ViewDataAuths
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
            sub.Status, sub.HasNg,
            sub.OperatorSignedAt, sub.LeaderSignedAt, sub.KasubsieSignedAt, sub.KasieSignedAt,
            sub.LeaderMemo,
            sub.LeaderApprovalType,
            Form = sub.Form != null ? new { sub.Form.Id, sub.Form.Code, sub.Form.Title, sub.Form.Subtitle } : null,
            CheckValues = sub.CheckValues.Select(cv => new { cv.Id, cv.SettingKey, cv.Value }),
            Problems = sub.Problems.OrderBy(p => p.SortOrder).Select(p => new { p.Id, p.SortOrder, p.ValuesJson }),
            Signatures = sub.Signatures.Select(sig => new { sig.Id, sig.RoleKey, sig.SignatureData, sig.SignedAt }),
            Attachments = sub.ApprovalAttachments.Select(a => new { a.Id, a.RoleKey, a.FileName, a.ContentType, a.UploadedAt }),
        });
    }

    // POST api/formsubmission
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CosSubmission submission)
    {
        submission.CreatedAt = DateTime.UtcNow;
        submission.Status = "pending_leader";

        // Auto-detect NG values
        if (submission.CheckValues != null)
        {
            submission.HasNg = submission.CheckValues.Any(cv => cv.Value == "ng");
        }

        // If operator signature is present, set operatorSignedAt
        if (submission.Signatures != null && submission.Signatures.Any(s => s.RoleKey == "operator" && !string.IsNullOrEmpty(s.SignatureData)))
        {
            submission.OperatorSignedAt = DateTime.UtcNow;
        }

        if (submission.CheckValues != null)
            foreach (var cv in submission.CheckValues) cv.SubmissionId = 0;
        if (submission.Problems != null)
            foreach (var p in submission.Problems) p.SubmissionId = 0;
        if (submission.Signatures != null)
        {
            foreach (var s in submission.Signatures)
            {
                s.SubmissionId = 0;
                if (!string.IsNullOrEmpty(s.SignatureData))
                    s.SignedAt = DateTime.UtcNow;
            }
        }

        _cosDb.CosSubmissions.Add(submission);
        await _cosDb.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = submission.Id }, new { id = submission.Id });
    }


    // PUT api/formsubmission/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CosSubmission input)
    {
        var existing = await _cosDb.CosSubmissions
            .Include(s => s.CheckValues)
            .Include(s => s.Problems)
            .Include(s => s.Signatures)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (existing == null) return NotFound();

        // Update header fields
        existing.FormId = input.FormId;
        existing.Tanggal = input.Tanggal;
        existing.LineId = input.LineId;
        existing.ShiftId = input.ShiftId;
        existing.OperatorEmpId = input.OperatorEmpId;
        existing.LeaderEmpId = input.LeaderEmpId;
        existing.KasubsieEmpId = input.KasubsieEmpId;
        existing.KasieEmpId = input.KasieEmpId;
        existing.BatterySlotsJson = input.BatterySlotsJson;
        existing.UpdatedAt = DateTime.UtcNow;

        // Replace check values
        _cosDb.CosCheckValues.RemoveRange(existing.CheckValues);
        if (input.CheckValues != null)
        {
            foreach (var cv in input.CheckValues)
            {
                cv.Id = 0;
                cv.SubmissionId = existing.Id;
                _cosDb.CosCheckValues.Add(cv);
            }
        }

        // Replace problems
        _cosDb.CosProblems.RemoveRange(existing.Problems);
        if (input.Problems != null)
        {
            foreach (var p in input.Problems)
            {
                p.Id = 0;
                p.SubmissionId = existing.Id;
                _cosDb.CosProblems.Add(p);
            }
        }

        // Replace signatures
        _cosDb.CosSignatureEntries.RemoveRange(existing.Signatures);
        if (input.Signatures != null)
        {
            foreach (var s in input.Signatures)
            {
                s.Id = 0;
                s.SubmissionId = existing.Id;
                _cosDb.CosSignatureEntries.Add(s);
            }
        }

        await _cosDb.SaveChangesAsync();
        return Ok(new { id = existing.Id });
    }


    // DELETE api/formsubmission/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sub = await _cosDb.CosSubmissions.FindAsync(id);
        if (sub == null) return NotFound();
        _cosDb.CosSubmissions.Remove(sub);
        await _cosDb.SaveChangesAsync();
        return NoContent();
    }


    // PUT api/formsubmission/5/approve
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> Approve(int id, [FromBody] ApproveRequest request)
    {
        var sub = await _cosDb.CosSubmissions
            .Include(s => s.Signatures)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sub == null) return NotFound();

        var now = DateTime.UtcNow;

        // Validate role matches current status
        var expectedRole = sub.Status switch
        {
            "pending_leader" => "leader",
            "pending_kasubsie" => "kasubsie",
            "pending_kasie" => "kasie",
            _ => null,
        };

        if (expectedRole == null || expectedRole != request.Role)
            return BadRequest(new { error = $"Cannot approve as '{request.Role}'. Current status: {sub.Status}" });

        // Save signature entry
        var existingSig = sub.Signatures.FirstOrDefault(s => s.RoleKey == request.Role);
        if (existingSig != null)
        {
            existingSig.SignatureData = request.SignatureData;
            existingSig.SignedAt = now;
        }
        else
        {
            _cosDb.CosSignatureEntries.Add(new CosSignatureEntry
            {
                SubmissionId = id,
                RoleKey = request.Role,
                SignatureData = request.SignatureData,
                SignedAt = now,
            });
        }

        // Update timestamps and memos per role
        switch (request.Role)
        {
            case "leader":
                sub.LeaderSignedAt = now;
                sub.LeaderMemo = request.Memo;
                sub.Status = "pending_kasubsie";
                break;
            case "kasubsie":
                sub.KasubsieSignedAt = now;
                sub.Status = "pending_kasie";
                break;
            case "kasie":
                sub.KasieSignedAt = now;
                sub.Status = "completed";
                break;
        }

        sub.UpdatedAt = now;
        await _cosDb.SaveChangesAsync();

        return Ok(new { status = sub.Status });
    }


    // POST api/formsubmission/5/upload-memo
    [HttpPost("{id}/upload-memo")]
    public async Task<IActionResult> UploadMemo(int id, [FromForm] string role, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "File is required." });

        var sub = await _cosDb.CosSubmissions.FindAsync(id);
        if (sub == null) return NotFound();

        // Create upload directory
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "memos");
        Directory.CreateDirectory(uploadsDir);

        // Generate unique filename
        var ext = Path.GetExtension(file.FileName);
        var uniqueName = $"{id}_{role}_{DateTime.UtcNow:yyyyMMddHHmmss}{ext}";
        var filePath = Path.Combine(uploadsDir, uniqueName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Save attachment record
        var attachment = new CosApprovalAttachment
        {
            SubmissionId = id,
            RoleKey = role,
            FileName = file.FileName,
            FilePath = $"/uploads/memos/{uniqueName}",
            ContentType = file.ContentType,
            UploadedAt = DateTime.UtcNow,
        };
        _cosDb.CosApprovalAttachments.Add(attachment);

        // Update leader approval type if applicable
        if (role == "leader")
        {
            sub.LeaderApprovalType = "memo_upload";
        }

        await _cosDb.SaveChangesAsync();

        return Ok(new
        {
            attachment.Id,
            attachment.FileName,
            attachment.FilePath,
            attachment.ContentType,
            attachment.UploadedAt,
        });
    }


    // DELETE api/formsubmission/attachment/5
    [HttpDelete("attachment/{attachmentId}")]
    public async Task<IActionResult> DeleteAttachment(int attachmentId)
    {
        var att = await _cosDb.CosApprovalAttachments.FindAsync(attachmentId);
        if (att == null) return NotFound();

        // Delete physical file
        var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", att.FilePath.TrimStart('/'));
        if (System.IO.File.Exists(physicalPath))
            System.IO.File.Delete(physicalPath);

        _cosDb.CosApprovalAttachments.Remove(att);
        await _cosDb.SaveChangesAsync();
        return NoContent();
    }
}


// ── Request DTOs ──
public class ApproveRequest
{
    public string Role { get; set; } = null!;
    public string? SignatureData { get; set; }
    public string? Memo { get; set; }
}
