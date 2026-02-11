using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeSignatureController : ControllerBase
{
    private readonly FormCosDbContext _db;
    public EmployeeSignatureController(FormCosDbContext db) => _db = db;

    /// <summary>GET /api/employeesignature/{empId} — get signature for an employee</summary>
    [HttpGet("{empId}")]
    public async Task<IActionResult> Get(string empId)
    {
        var sig = await _db.CosEmployeeSignatures
            .FirstOrDefaultAsync(s => s.EmpId == empId);
        if (sig == null)
            return Ok(new { empId, signatureData = (string?)null });
        return Ok(new { sig.EmpId, sig.SignatureData });
    }

    /// <summary>GET /api/employeesignature/batch?empIds=A&empIds=B — get signatures for multiple employees</summary>
    [HttpGet("batch")]
    public async Task<IActionResult> GetBatch([FromQuery] string[] empIds)
    {
        if (empIds == null || empIds.Length == 0)
            return Ok(Array.Empty<object>());

        var sigs = await _db.CosEmployeeSignatures
            .Where(s => empIds.Contains(s.EmpId))
            .Select(s => new { s.EmpId, s.SignatureData })
            .ToListAsync();
        return Ok(sigs);
    }

    /// <summary>PUT /api/employeesignature/{empId} — save/update signature for an employee</summary>
    [HttpPut("{empId}")]
    public async Task<IActionResult> Save(string empId, [FromBody] SaveSignatureDto dto)
    {
        var existing = await _db.CosEmployeeSignatures
            .FirstOrDefaultAsync(s => s.EmpId == empId);

        if (existing != null)
        {
            existing.SignatureData = dto.SignatureData;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _db.CosEmployeeSignatures.Add(new CosEmployeeSignature
            {
                EmpId = empId,
                SignatureData = dto.SignatureData,
                UpdatedAt = DateTime.UtcNow,
            });
        }

        await _db.SaveChangesAsync();
        return Ok(new { empId, saved = true });
    }
}

public class SaveSignatureDto
{
    public string? SignatureData { get; set; }
}
