using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using backend.Data;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly FormCosDbContext _db;
    public AdminController(FormCosDbContext db) => _db = db;

    // helper: read optional properties from JsonElement
    private static string? GetStr(JsonElement el, string prop) =>
        el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
    private static int? GetInt(JsonElement el, string prop) =>
        el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetInt32() : null;
    private static decimal? GetDec(JsonElement el, string prop)
    {
        if (!el.TryGetProperty(prop, out var v)) return null;
        if (v.ValueKind == JsonValueKind.Null) return null;
        if (v.ValueKind == JsonValueKind.Number) return v.GetDecimal();
        return null;
    }
    private static bool Has(JsonElement el, string prop) => el.TryGetProperty(prop, out _);

    // ═══════════ CHECK ITEMS ═══════════

    [HttpGet("checkitems")]
    public async Task<IActionResult> GetCheckItems([FromQuery] int formId)
    {
        var items = await _db.CosCheckItems
            .Where(c => c.FormId == formId)
            .Include(c => c.SubRows.OrderBy(s => s.SortOrder))
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost("checkitems")]
    public async Task<IActionResult> CreateCheckItem([FromBody] CosCheckItem item)
    {
        _db.CosCheckItems.Add(item);
        await _db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpPut("checkitems/{id}")]
    public async Task<IActionResult> UpdateCheckItem(int id, [FromBody] JsonElement body)
    {
        var e = await _db.CosCheckItems.FindAsync(id);
        if (e == null) return NotFound();

        if (Has(body, "itemKey"))          e.ItemKey = GetStr(body, "itemKey") ?? e.ItemKey;
        if (Has(body, "label"))            e.Label = GetStr(body, "label") ?? e.Label;
        if (Has(body, "type"))             e.Type = GetStr(body, "type") ?? e.Type;
        if (Has(body, "visualStandard"))   e.VisualStandard = GetStr(body, "visualStandard");
        if (Has(body, "numericStdKey"))    e.NumericStdKey = GetStr(body, "numericStdKey");
        if (Has(body, "fixedStandard"))    e.FixedStandard = GetStr(body, "fixedStandard");
        if (Has(body, "fixedMin"))         e.FixedMin = GetDec(body, "fixedMin");
        if (Has(body, "fixedMax"))         e.FixedMax = GetDec(body, "fixedMax");
        if (Has(body, "frequency"))        e.Frequency = GetStr(body, "frequency");
        if (Has(body, "keterangan"))       e.Keterangan = GetStr(body, "keterangan");
        if (Has(body, "conditionalLabel")) e.ConditionalLabel = GetStr(body, "conditionalLabel");
        if (Has(body, "sortOrder"))        e.SortOrder = GetInt(body, "sortOrder") ?? e.SortOrder;

        await _db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpDelete("checkitems/{id}")]
    public async Task<IActionResult> DeleteCheckItem(int id)
    {
        var e = await _db.CosCheckItems.FindAsync(id);
        if (e == null) return NotFound();
        _db.CosCheckItems.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ═══════════ SUB ROWS ═══════════

    [HttpPost("subrows")]
    public async Task<IActionResult> CreateSubRow([FromBody] CosCheckSubRow row)
    {
        _db.CosCheckSubRows.Add(row);
        await _db.SaveChangesAsync();
        return Ok(row);
    }

    [HttpPut("subrows/{id}")]
    public async Task<IActionResult> UpdateSubRow(int id, [FromBody] JsonElement body)
    {
        var e = await _db.CosCheckSubRows.FindAsync(id);
        if (e == null) return NotFound();

        if (Has(body, "suffix"))        e.Suffix = GetStr(body, "suffix") ?? e.Suffix;
        if (Has(body, "label"))         e.Label = GetStr(body, "label") ?? e.Label;
        if (Has(body, "fixedStandard")) e.FixedStandard = GetStr(body, "fixedStandard");
        if (Has(body, "fixedMin"))      e.FixedMin = GetDec(body, "fixedMin");
        if (Has(body, "fixedMax"))      e.FixedMax = GetDec(body, "fixedMax");
        if (Has(body, "sortOrder"))     e.SortOrder = GetInt(body, "sortOrder") ?? e.SortOrder;

        await _db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpDelete("subrows/{id}")]
    public async Task<IActionResult> DeleteSubRow(int id)
    {
        var e = await _db.CosCheckSubRows.FindAsync(id);
        if (e == null) return NotFound();
        _db.CosCheckSubRows.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ═══════════ PROBLEM COLUMNS ═══════════

    [HttpGet("problemcolumns")]
    public async Task<IActionResult> GetProblemColumns([FromQuery] int formId)
    {
        var cols = await _db.CosProblemColumns
            .Where(c => c.FormId == formId)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
        return Ok(cols);
    }

    [HttpPost("problemcolumns")]
    public async Task<IActionResult> CreateProblemColumn([FromBody] CosProblemColumn col)
    {
        _db.CosProblemColumns.Add(col);
        await _db.SaveChangesAsync();
        return Ok(col);
    }

    [HttpPut("problemcolumns/{id}")]
    public async Task<IActionResult> UpdateProblemColumn(int id, [FromBody] JsonElement body)
    {
        var e = await _db.CosProblemColumns.FindAsync(id);
        if (e == null) return NotFound();

        if (Has(body, "columnKey")) e.ColumnKey = GetStr(body, "columnKey") ?? e.ColumnKey;
        if (Has(body, "label"))     e.Label = GetStr(body, "label") ?? e.Label;
        if (Has(body, "fieldType")) e.FieldType = GetStr(body, "fieldType") ?? e.FieldType;
        if (Has(body, "width"))     e.Width = GetStr(body, "width") ?? e.Width;
        if (Has(body, "sortOrder")) e.SortOrder = GetInt(body, "sortOrder") ?? e.SortOrder;

        await _db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpDelete("problemcolumns/{id}")]
    public async Task<IActionResult> DeleteProblemColumn(int id)
    {
        var e = await _db.CosProblemColumns.FindAsync(id);
        if (e == null) return NotFound();
        _db.CosProblemColumns.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ═══════════ SIGNATURE SLOTS ═══════════

    [HttpGet("signatureslots")]
    public async Task<IActionResult> GetSignatureSlots([FromQuery] int formId)
    {
        var slots = await _db.CosSignatureSlots
            .Where(s => s.FormId == formId)
            .OrderBy(s => s.SortOrder)
            .ToListAsync();
        return Ok(slots);
    }

    [HttpPost("signatureslots")]
    public async Task<IActionResult> CreateSignatureSlot([FromBody] CosSignatureSlot slot)
    {
        _db.CosSignatureSlots.Add(slot);
        await _db.SaveChangesAsync();
        return Ok(slot);
    }

    [HttpPut("signatureslots/{id}")]
    public async Task<IActionResult> UpdateSignatureSlot(int id, [FromBody] JsonElement body)
    {
        var e = await _db.CosSignatureSlots.FindAsync(id);
        if (e == null) return NotFound();

        if (Has(body, "roleKey"))   e.RoleKey = GetStr(body, "roleKey") ?? e.RoleKey;
        if (Has(body, "label"))     e.Label = GetStr(body, "label") ?? e.Label;
        if (Has(body, "sortOrder")) e.SortOrder = GetInt(body, "sortOrder") ?? e.SortOrder;

        await _db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpDelete("signatureslots/{id}")]
    public async Task<IActionResult> DeleteSignatureSlot(int id)
    {
        var e = await _db.CosSignatureSlots.FindAsync(id);
        if (e == null) return NotFound();
        _db.CosSignatureSlots.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ═══════════ BATTERY TYPES ═══════════

    [HttpGet("batterytypes")]
    public async Task<IActionResult> GetBatteryTypes()
    {
        var types = await _db.CosBatteryTypes
            .Include(t => t.Standards)
            .OrderBy(t => t.Name)
            .Select(t => new
            {
                t.Id, t.Name,
                Standards = t.Standards.OrderBy(s => s.ParamKey).Select(s => new { s.Id, s.ParamKey, s.Value, s.MinValue, s.MaxValue, s.BatteryTypeId }),
            })
            .ToListAsync();
        return Ok(types);
    }

    [HttpPost("batterytypes")]
    public async Task<IActionResult> CreateBatteryType([FromBody] CosBatteryType type)
    {
        _db.CosBatteryTypes.Add(type);
        await _db.SaveChangesAsync();
        return Ok(new { type.Id, type.Name });
    }

    [HttpPut("batterytypes/{id}")]
    public async Task<IActionResult> UpdateBatteryType(int id, [FromBody] JsonElement body)
    {
        var e = await _db.CosBatteryTypes.FindAsync(id);
        if (e == null) return NotFound();
        if (Has(body, "name")) e.Name = GetStr(body, "name") ?? e.Name;
        await _db.SaveChangesAsync();
        return Ok(new { e.Id, e.Name });
    }

    [HttpDelete("batterytypes/{id}")]
    public async Task<IActionResult> DeleteBatteryType(int id)
    {
        var e = await _db.CosBatteryTypes.FindAsync(id);
        if (e == null) return NotFound();
        _db.CosBatteryTypes.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ═══════════ BATTERY STANDARDS ═══════════

    [HttpGet("batterystandards")]
    public async Task<IActionResult> GetBatteryStandards([FromQuery] int? batteryTypeId)
    {
        var query = _db.CosBatteryStandards.AsQueryable();
        if (batteryTypeId.HasValue) query = query.Where(s => s.BatteryTypeId == batteryTypeId.Value);
        return Ok(await query.ToListAsync());
    }

    [HttpPost("batterystandards")]
    public async Task<IActionResult> CreateBatteryStandard([FromBody] CosBatteryStandard std)
    {
        _db.CosBatteryStandards.Add(std);
        await _db.SaveChangesAsync();
        return Ok(std);
    }

    [HttpPut("batterystandards/{id}")]
    public async Task<IActionResult> UpdateBatteryStandard(int id, [FromBody] JsonElement body)
    {
        var e = await _db.CosBatteryStandards.FindAsync(id);
        if (e == null) return NotFound();

        if (Has(body, "paramKey"))      e.ParamKey = GetStr(body, "paramKey") ?? e.ParamKey;
        if (Has(body, "value"))         e.Value = GetStr(body, "value") ?? e.Value;
        if (Has(body, "minValue"))      e.MinValue = GetDec(body, "minValue");
        if (Has(body, "maxValue"))      e.MaxValue = GetDec(body, "maxValue");
        if (Has(body, "batteryTypeId")) e.BatteryTypeId = GetInt(body, "batteryTypeId") ?? e.BatteryTypeId;

        await _db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpDelete("batterystandards/{id}")]
    public async Task<IActionResult> DeleteBatteryStandard(int id)
    {
        var e = await _db.CosBatteryStandards.FindAsync(id);
        if (e == null) return NotFound();
        _db.CosBatteryStandards.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ═══════════ MOLDS (Master Data — read from tlkp_mold) ═══════════

    [HttpGet("molds")]
    public async Task<IActionResult> GetMolds()
    {
        var molds = await _db.TlkpMolds
            .OrderBy(m => m.MoldCode)
            .Select(m => new { m.MoldCode, m.MoldDescription, m.MoldStatus, m.IdSection })
            .ToListAsync();
        return Ok(molds);
    }

    // ═══════════ PERSONNEL (read-only, from master data) ═══════════

    [HttpGet("personnel")]
    public async Task<IActionResult> GetPersonnel()
    {
        var operators = await (
            from op in _db.TlkpOperators
            join auth in _db.ViewDataAuths on op.UserId equals auth.EmpId into authJoin
            from a in authJoin.DefaultIfEmpty()
            select new { empId = op.UserId, name = a != null ? a.FullName : op.UserId, empNo = a != null ? a.EmpNo : null, lgpId = op.LgpId, groupId = op.GroupId }
        ).ToListAsync();

        var leaderEmpIds = await _db.TlkpLineGroups
            .Where(lg => lg.LgpLeader != null).Select(lg => lg.LgpLeader!).Distinct().ToListAsync();
        var leaders = await _db.ViewDataAuths
            .Where(a => leaderEmpIds.Contains(a.EmpId))
            .Select(a => new { empId = a.EmpId, name = a.FullName, empNo = a.EmpNo }).ToListAsync();

        var kasubsieEmpIds = await _db.TlkpLineGroups
            .Where(lg => lg.LgpKasubsie != null).Select(lg => lg.LgpKasubsie!).Distinct().ToListAsync();
        var kasubsies = await _db.ViewDataAuths
            .Where(a => kasubsieEmpIds.Contains(a.EmpId))
            .Select(a => new { empId = a.EmpId, name = a.FullName, empNo = a.EmpNo }).ToListAsync();

        var kasieEmpIds = await _db.TlkpUserKasies
            .Select(uk => uk.KasieEmpId).Distinct().ToListAsync();
        var kasies = await _db.ViewDataAuths
            .Where(a => kasieEmpIds.Contains(a.EmpId))
            .Select(a => new { empId = a.EmpId, name = a.FullName, empNo = a.EmpNo }).ToListAsync();

        return Ok(new { operators, leaders, kasubsies, kasies });
    }
}
