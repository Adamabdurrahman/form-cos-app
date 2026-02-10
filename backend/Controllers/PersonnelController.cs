using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonnelController : ControllerBase
{
    private readonly FormCosDbContext _db;
    public PersonnelController(FormCosDbContext db) => _db = db;

    // GET api/personnel/operators
    [HttpGet("operators")]
    public async Task<IActionResult> GetOperators()
    {
        var operators = await (
            from op in _db.TlkpOperators
            join auth in _db.ViewDataAuths on op.UserId equals auth.EmpId into authJoin
            from a in authJoin.DefaultIfEmpty()
            select new { empId = op.UserId, name = a != null ? a.FullName : op.UserId, empNo = a != null ? a.EmpNo : null, lgpId = op.LgpId, groupId = op.GroupId }
        ).ToListAsync();
        return Ok(operators);
    }

    // GET api/personnel/leaders
    [HttpGet("leaders")]
    public async Task<IActionResult> GetLeaders()
    {
        var leaderEmpIds = await _db.TlkpLineGroups
            .Where(lg => lg.LgpLeader != null)
            .Select(lg => lg.LgpLeader!)
            .Distinct().ToListAsync();

        var leaders = await _db.ViewDataAuths
            .Where(a => leaderEmpIds.Contains(a.EmpId))
            .Select(a => new { empId = a.EmpId, name = a.FullName, empNo = a.EmpNo })
            .ToListAsync();

        // include any emp_ids not found in ViewDataAuth
        var foundIds = leaders.Select(l => l.empId).ToHashSet();
        var missing = leaderEmpIds.Where(e => !foundIds.Contains(e))
            .Select(e => new { empId = e, name = e, empNo = (string?)null });

        return Ok(leaders.Concat(missing));
    }

    // GET api/personnel/kasubsies
    [HttpGet("kasubsies")]
    public async Task<IActionResult> GetKasubsies()
    {
        var empIds = await _db.TlkpLineGroups
            .Where(lg => lg.LgpKasubsie != null)
            .Select(lg => lg.LgpKasubsie!)
            .Distinct().ToListAsync();

        var kasubsies = await _db.ViewDataAuths
            .Where(a => empIds.Contains(a.EmpId))
            .Select(a => new { empId = a.EmpId, name = a.FullName, empNo = a.EmpNo })
            .ToListAsync();

        var foundIds = kasubsies.Select(k => k.empId).ToHashSet();
        var missing = empIds.Where(e => !foundIds.Contains(e))
            .Select(e => new { empId = e, name = e, empNo = (string?)null });

        return Ok(kasubsies.Concat(missing));
    }

    // GET api/personnel/kasies
    [HttpGet("kasies")]
    public async Task<IActionResult> GetKasies()
    {
        var empIds = await _db.TlkpUserKasies
            .Where(uk => uk.KasieEmpId != null)
            .Select(uk => uk.KasieEmpId!)
            .Distinct().ToListAsync();

        var kasies = await _db.ViewDataAuths
            .Where(a => empIds.Contains(a.EmpId))
            .Select(a => new { empId = a.EmpId, name = a.FullName, empNo = a.EmpNo })
            .ToListAsync();

        var foundIds = kasies.Select(k => k.empId).ToHashSet();
        var missing = empIds.Where(e => !foundIds.Contains(e))
            .Select(e => new { empId = e, name = e, empNo = (string?)null });

        return Ok(kasies.Concat(missing));
    }

    // GET api/personnel/hierarchy/{operatorEmpId}
    [HttpGet("hierarchy/{operatorEmpId}")]
    public async Task<IActionResult> GetHierarchy(string operatorEmpId)
    {
        var op = await _db.TlkpOperators.FirstOrDefaultAsync(o => o.UserId == operatorEmpId);
        if (op == null) return NotFound("Operator not found");

        var lineGroup = await _db.TlkpLineGroups.FirstOrDefaultAsync(lg => lg.LgpId == op.LgpId);

        string? leaderEmpId = lineGroup?.LgpLeader;
        string? kasubsieEmpId = lineGroup?.LgpKasubsie;
        string? kasieEmpId = null;

        if (lineGroup?.UserKasieId != null)
        {
            var userKasie = await _db.TlkpUserKasies.FirstOrDefaultAsync(uk => uk.UserKasieId == lineGroup.UserKasieId);
            kasieEmpId = userKasie?.KasieEmpId;
        }

        // Resolve all names
        var allEmpIds = new[] { operatorEmpId, leaderEmpId, kasubsieEmpId, kasieEmpId }
            .Where(e => e != null).Cast<string>().Distinct().ToList();

        var authLookup = await _db.ViewDataAuths
            .Where(a => allEmpIds.Contains(a.EmpId))
            .ToDictionaryAsync(a => a.EmpId, a => a.FullName);

        string ResolveName(string? empId) => empId != null && authLookup.TryGetValue(empId, out var n) ? n : empId ?? "";

        return Ok(new
        {
            operatorEmpId,
            operatorName = ResolveName(operatorEmpId),
            leaderEmpId,
            leaderName = ResolveName(leaderEmpId),
            kasubsieEmpId,
            kasubsieName = ResolveName(kasubsieEmpId),
            kasieEmpId,
            kasieName = ResolveName(kasieEmpId)
        });
    }
}
