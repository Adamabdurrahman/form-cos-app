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
    private static bool? GetBool(JsonElement el, string prop) =>
        el.TryGetProperty(prop, out var v) && (v.ValueKind == JsonValueKind.True || v.ValueKind == JsonValueKind.False) ? v.GetBoolean() : null;
    private static bool Has(JsonElement el, string prop) => el.TryGetProperty(prop, out _);

    // ═══════════ CHECK ITEMS ═══════════

    [HttpGet("checkitems")]
    public async Task<IActionResult> GetCheckItems([FromQuery] int formId)
    {
        var items = await _db.CheckItems
            .Where(c => c.FormId == formId)
            .Include(c => c.SubRows.OrderBy(s => s.SortOrder))
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost("checkitems")]
    public async Task<IActionResult> CreateCheckItem([FromBody] CheckItem item)
    {
        _db.CheckItems.Add(item);
        await _db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpPut("checkitems/{id}")]
    public async Task<IActionResult> UpdateCheckItem(int id, [FromBody] JsonElement body)
    {
        var e = await _db.CheckItems.FindAsync(id);
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
        var e = await _db.CheckItems.FindAsync(id);
        if (e == null) return NotFound();
        _db.CheckItems.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ═══════════ SUB ROWS ═══════════

    [HttpPost("subrows")]
    public async Task<IActionResult> CreateSubRow([FromBody] CheckSubRow row)
    {
        _db.CheckSubRows.Add(row);
        await _db.SaveChangesAsync();
        return Ok(row);
    }

    [HttpPut("subrows/{id}")]
    public async Task<IActionResult> UpdateSubRow(int id, [FromBody] JsonElement body)
    {
        var e = await _db.CheckSubRows.FindAsync(id);
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
        var e = await _db.CheckSubRows.FindAsync(id);
        if (e == null) return NotFound();
        _db.CheckSubRows.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ═══════════ PROBLEM COLUMNS ═══════════

    [HttpGet("problemcolumns")]
    public async Task<IActionResult> GetProblemColumns([FromQuery] int formId)
    {
        var cols = await _db.FormProblemColumns
            .Where(c => c.FormId == formId)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
        return Ok(cols);
    }

    [HttpPost("problemcolumns")]
    public async Task<IActionResult> CreateProblemColumn([FromBody] FormProblemColumn col)
    {
        _db.FormProblemColumns.Add(col);
        await _db.SaveChangesAsync();
        return Ok(col);
    }

    [HttpPut("problemcolumns/{id}")]
    public async Task<IActionResult> UpdateProblemColumn(int id, [FromBody] JsonElement body)
    {
        var e = await _db.FormProblemColumns.FindAsync(id);
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
        var e = await _db.FormProblemColumns.FindAsync(id);
        if (e == null) return NotFound();
        _db.FormProblemColumns.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ═══════════ SIGNATURE SLOTS ═══════════

    [HttpGet("signatureslots")]
    public async Task<IActionResult> GetSignatureSlots([FromQuery] int formId)
    {
        var slots = await _db.FormSignatureSlots
            .Where(s => s.FormId == formId)
            .OrderBy(s => s.SortOrder)
            .ToListAsync();
        return Ok(slots);
    }

    [HttpPost("signatureslots")]
    public async Task<IActionResult> CreateSignatureSlot([FromBody] FormSignatureSlot slot)
    {
        _db.FormSignatureSlots.Add(slot);
        await _db.SaveChangesAsync();
        return Ok(slot);
    }

    [HttpPut("signatureslots/{id}")]
    public async Task<IActionResult> UpdateSignatureSlot(int id, [FromBody] JsonElement body)
    {
        var e = await _db.FormSignatureSlots.FindAsync(id);
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
        var e = await _db.FormSignatureSlots.FindAsync(id);
        if (e == null) return NotFound();
        _db.FormSignatureSlots.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ═══════════ BATTERY TYPES ═══════════

    [HttpGet("batterytypes")]
    public async Task<IActionResult> GetBatteryTypes()
    {
        var types = await _db.BatteryTypes
            .Include(t => t.Molds)
            .Include(t => t.Standards)
            .OrderBy(t => t.Name)
            .Select(t => new
            {
                t.Id, t.Name,
                Molds = t.Molds.OrderBy(m => m.Name).Select(m => new { m.Id, m.Name, m.BatteryTypeId }),
                Standards = t.Standards.OrderBy(s => s.ParamKey).Select(s => new { s.Id, s.ParamKey, s.Value, s.MinValue, s.MaxValue, s.BatteryTypeId }),
            })
            .ToListAsync();
        return Ok(types);
    }

    [HttpPost("batterytypes")]
    public async Task<IActionResult> CreateBatteryType([FromBody] BatteryType type)
    {
        _db.BatteryTypes.Add(type);
        await _db.SaveChangesAsync();
        return Ok(new { type.Id, type.Name });
    }

    [HttpPut("batterytypes/{id}")]
    public async Task<IActionResult> UpdateBatteryType(int id, [FromBody] JsonElement body)
    {
        var e = await _db.BatteryTypes.FindAsync(id);
        if (e == null) return NotFound();
        if (Has(body, "name")) e.Name = GetStr(body, "name") ?? e.Name;
        await _db.SaveChangesAsync();
        return Ok(new { e.Id, e.Name });
    }

    [HttpDelete("batterytypes/{id}")]
    public async Task<IActionResult> DeleteBatteryType(int id)
    {
        var e = await _db.BatteryTypes.FindAsync(id);
        if (e == null) return NotFound();
        _db.BatteryTypes.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ═══════════ BATTERY STANDARDS ═══════════

    [HttpGet("batterystandards")]
    public async Task<IActionResult> GetBatteryStandards([FromQuery] int? batteryTypeId)
    {
        var query = _db.BatteryStandards.AsQueryable();
        if (batteryTypeId.HasValue) query = query.Where(s => s.BatteryTypeId == batteryTypeId.Value);
        return Ok(await query.ToListAsync());
    }

    [HttpPost("batterystandards")]
    public async Task<IActionResult> CreateBatteryStandard([FromBody] BatteryStandard std)
    {
        _db.BatteryStandards.Add(std);
        await _db.SaveChangesAsync();
        return Ok(std);
    }

    [HttpPut("batterystandards/{id}")]
    public async Task<IActionResult> UpdateBatteryStandard(int id, [FromBody] JsonElement body)
    {
        var e = await _db.BatteryStandards.FindAsync(id);
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
        var e = await _db.BatteryStandards.FindAsync(id);
        if (e == null) return NotFound();
        _db.BatteryStandards.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ═══════════ BATTERY MOLDS ═══════════

    [HttpPost("batterymolds")]
    public async Task<IActionResult> CreateBatteryMold([FromBody] BatteryMold mold)
    {
        _db.BatteryMolds.Add(mold);
        await _db.SaveChangesAsync();
        return Ok(mold);
    }

    [HttpPut("batterymolds/{id}")]
    public async Task<IActionResult> UpdateBatteryMold(int id, [FromBody] JsonElement body)
    {
        var e = await _db.BatteryMolds.FindAsync(id);
        if (e == null) return NotFound();
        if (Has(body, "name")) e.Name = GetStr(body, "name") ?? e.Name;
        if (Has(body, "batteryTypeId")) e.BatteryTypeId = GetInt(body, "batteryTypeId") ?? e.BatteryTypeId;
        await _db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpDelete("batterymolds/{id}")]
    public async Task<IActionResult> DeleteBatteryMold(int id)
    {
        var e = await _db.BatteryMolds.FindAsync(id);
        if (e == null) return NotFound();
        _db.BatteryMolds.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ═══════════ PERSONNEL ═══════════

    [HttpGet("personnel")]
    public async Task<IActionResult> GetPersonnel()
    {
        var kasies = await _db.Kasies.OrderBy(k => k.Name).ToListAsync();
        var kasubsies = await _db.Kasubsies.OrderBy(k => k.Name).ToListAsync();
        var leaders = await _db.Leaders.OrderBy(l => l.Name).ToListAsync();
        var operators = await _db.Operators.OrderBy(o => o.Name).ToListAsync();
        return Ok(new { kasies, kasubsies, leaders, operators });
    }

    // ── Operators ──
    [HttpPost("operators")]
    public async Task<IActionResult> CreateOperator([FromBody] Operator op)
    {
        _db.Operators.Add(op);
        await _db.SaveChangesAsync();
        return Ok(op);
    }

    [HttpPut("operators/{id}")]
    public async Task<IActionResult> UpdateOperator(int id, [FromBody] JsonElement body)
    {
        var e = await _db.Operators.FindAsync(id);
        if (e == null) return NotFound();
        if (Has(body, "name")) e.Name = GetStr(body, "name") ?? e.Name;
        if (Has(body, "leaderId")) e.LeaderId = GetInt(body, "leaderId") ?? e.LeaderId;
        await _db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpDelete("operators/{id}")]
    public async Task<IActionResult> DeleteOperator(int id)
    {
        var e = await _db.Operators.FindAsync(id);
        if (e == null) return NotFound();
        _db.Operators.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ── Leaders ──
    [HttpPost("leaders")]
    public async Task<IActionResult> CreateLeader([FromBody] Leader leader)
    {
        _db.Leaders.Add(leader);
        await _db.SaveChangesAsync();
        return Ok(leader);
    }

    [HttpPut("leaders/{id}")]
    public async Task<IActionResult> UpdateLeader(int id, [FromBody] JsonElement body)
    {
        var e = await _db.Leaders.FindAsync(id);
        if (e == null) return NotFound();
        if (Has(body, "name")) e.Name = GetStr(body, "name") ?? e.Name;
        if (Has(body, "kasubsieId")) e.KasubsieId = GetInt(body, "kasubsieId") ?? e.KasubsieId;
        await _db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpDelete("leaders/{id}")]
    public async Task<IActionResult> DeleteLeader(int id)
    {
        var e = await _db.Leaders.FindAsync(id);
        if (e == null) return NotFound();
        _db.Leaders.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ── Kasubsie ──
    [HttpPost("kasubsies")]
    public async Task<IActionResult> CreateKasubsie([FromBody] Kasubsie ks)
    {
        _db.Kasubsies.Add(ks);
        await _db.SaveChangesAsync();
        return Ok(ks);
    }

    [HttpPut("kasubsies/{id}")]
    public async Task<IActionResult> UpdateKasubsie(int id, [FromBody] JsonElement body)
    {
        var e = await _db.Kasubsies.FindAsync(id);
        if (e == null) return NotFound();
        if (Has(body, "name")) e.Name = GetStr(body, "name") ?? e.Name;
        if (Has(body, "kasieId")) e.KasieId = GetInt(body, "kasieId") ?? e.KasieId;
        await _db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpDelete("kasubsies/{id}")]
    public async Task<IActionResult> DeleteKasubsie(int id)
    {
        var e = await _db.Kasubsies.FindAsync(id);
        if (e == null) return NotFound();
        _db.Kasubsies.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ── Kasie ──
    [HttpPost("kasies")]
    public async Task<IActionResult> CreateKasie([FromBody] Kasie kasie)
    {
        _db.Kasies.Add(kasie);
        await _db.SaveChangesAsync();
        return Ok(kasie);
    }

    [HttpPut("kasies/{id}")]
    public async Task<IActionResult> UpdateKasie(int id, [FromBody] JsonElement body)
    {
        var e = await _db.Kasies.FindAsync(id);
        if (e == null) return NotFound();
        if (Has(body, "name")) e.Name = GetStr(body, "name") ?? e.Name;
        await _db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpDelete("kasies/{id}")]
    public async Task<IActionResult> DeleteKasie(int id)
    {
        var e = await _db.Kasies.FindAsync(id);
        if (e == null) return NotFound();
        _db.Kasies.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
