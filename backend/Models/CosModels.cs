using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

// ══════════════════════════════════════════════════
// COS FORM TABLES (new — created for COS Validation application)
// ══════════════════════════════════════════════════

// ─── Form Definition ───

[Table("cos_form_definitions")]
public class CosFormDefinition
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Required, Column("code"), MaxLength(50)]
    public string Code { get; set; } = null!;

    [Required, Column("title"), MaxLength(200)]
    public string Title { get; set; } = null!;

    [Column("subtitle"), MaxLength(300)]
    public string? Subtitle { get; set; }

    [Column("doc_number"), MaxLength(50)]
    public string? DocNumber { get; set; }

    [Column("revision"), MaxLength(20)]
    public string? Revision { get; set; }

    [Column("effective_date")]
    public DateTime? EffectiveDate { get; set; }

    [Column("slot_count")]
    public int SlotCount { get; set; } = 5;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<CosCheckItem> CheckItems { get; set; } = new List<CosCheckItem>();
    public ICollection<CosProblemColumn> ProblemColumns { get; set; } = new List<CosProblemColumn>();
    public ICollection<CosSignatureSlot> SignatureSlots { get; set; } = new List<CosSignatureSlot>();
}

// ─── Check Items ───

[Table("cos_check_items")]
public class CosCheckItem
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("form_id")]
    public int FormId { get; set; }

    [Required, Column("item_key"), MaxLength(100)]
    public string ItemKey { get; set; } = null!;

    [Required, Column("label"), MaxLength(300)]
    public string Label { get; set; } = null!;

    [Required, Column("type"), MaxLength(30)]
    public string Type { get; set; } = null!;  // visual, numeric, conditional

    [Column("visual_standard"), MaxLength(200)]
    public string? VisualStandard { get; set; }

    [Column("numeric_std_key"), MaxLength(100)]
    public string? NumericStdKey { get; set; }

    [Column("fixed_standard"), MaxLength(100)]
    public string? FixedStandard { get; set; }

    [Column("fixed_min", TypeName = "decimal(18,4)")]
    public decimal? FixedMin { get; set; }

    [Column("fixed_max", TypeName = "decimal(18,4)")]
    public decimal? FixedMax { get; set; }

    [Column("frequency"), MaxLength(100)]
    public string? Frequency { get; set; }

    [Column("keterangan"), MaxLength(300)]
    public string? Keterangan { get; set; }

    [Column("conditional_label"), MaxLength(200)]
    public string? ConditionalLabel { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }

    // Navigation
    [ForeignKey(nameof(FormId)), JsonIgnore]
    public CosFormDefinition? Form { get; set; }

    public ICollection<CosCheckSubRow> SubRows { get; set; } = new List<CosCheckSubRow>();
}

[Table("cos_check_sub_rows")]
public class CosCheckSubRow
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("check_item_id")]
    public int CheckItemId { get; set; }

    [Required, Column("suffix"), MaxLength(30)]
    public string Suffix { get; set; } = null!;

    [Required, Column("label"), MaxLength(200)]
    public string Label { get; set; } = null!;

    [Column("fixed_standard"), MaxLength(100)]
    public string? FixedStandard { get; set; }

    [Column("fixed_min", TypeName = "decimal(18,4)")]
    public decimal? FixedMin { get; set; }

    [Column("fixed_max", TypeName = "decimal(18,4)")]
    public decimal? FixedMax { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [ForeignKey(nameof(CheckItemId)), JsonIgnore]
    public CosCheckItem? CheckItem { get; set; }
}

// ─── Battery Types & Standards (COS-specific, not in master data) ───

[Table("cos_battery_types")]
public class CosBatteryType
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Required, Column("name"), MaxLength(100)]
    public string Name { get; set; } = null!;

    //tambah
    [MaxLength(255)]
    public string? SourceItemNum { get; set; }

    /// <summary>Optional link to tlkp_kategori.kat_id for master data reference</summary>
    [Column("kat_id")]
    public int? KatId { get; set; }

    /// <summary>FK to cos_form_definitions — determines which form this battery type uses</summary>
    [Column("form_id")]
    public int? FormId { get; set; }

    // Navigation
    [ForeignKey(nameof(FormId)), JsonIgnore]
    public CosFormDefinition? Form { get; set; }

    public ICollection<CosBatteryStandard> Standards { get; set; } = new List<CosBatteryStandard>();
}

[Table("cos_battery_standards")]
public class CosBatteryStandard
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("battery_type_id")]
    public int BatteryTypeId { get; set; }

    [Required, Column("param_key"), MaxLength(100)]
    public string ParamKey { get; set; } = null!;

    [Required, Column("value"), MaxLength(100)]
    public string Value { get; set; } = null!;

    [Column("min_value", TypeName = "decimal(18,4)")]
    public decimal? MinValue { get; set; }

    [Column("max_value", TypeName = "decimal(18,4)")]
    public decimal? MaxValue { get; set; }

    [ForeignKey(nameof(BatteryTypeId)), JsonIgnore]
    public CosBatteryType? BatteryType { get; set; }
}

// ─── Problem Columns ───

[Table("cos_problem_columns")]
public class CosProblemColumn
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("form_id")]
    public int FormId { get; set; }

    [Required, Column("column_key"), MaxLength(50)]
    public string ColumnKey { get; set; } = null!;

    [Required, Column("label"), MaxLength(100)]
    public string Label { get; set; } = null!;

    [Required, Column("field_type"), MaxLength(30)]
    public string FieldType { get; set; } = null!;

    [Required, Column("width"), MaxLength(20)]
    public string Width { get; set; } = null!;

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [ForeignKey(nameof(FormId)), JsonIgnore]
    public CosFormDefinition? Form { get; set; }
}

// ─── Signature Slots ───

[Table("cos_signature_slots")]
public class CosSignatureSlot
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("form_id")]
    public int FormId { get; set; }

    [Required, Column("role_key"), MaxLength(50)]
    public string RoleKey { get; set; } = null!;

    [Required, Column("label"), MaxLength(100)]
    public string Label { get; set; } = null!;

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [ForeignKey(nameof(FormId)), JsonIgnore]
    public CosFormDefinition? Form { get; set; }
}

// ─── Submissions ───

[Table("cos_submissions")]
public class CosSubmission
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("form_id")]
    public int FormId { get; set; }

    [Column("tanggal")]
    public DateTime Tanggal { get; set; }

    [Column("line_id")]
    public int? LineId { get; set; }

    [Column("shift_id")]
    public int? ShiftId { get; set; }

    /// <summary>Operator's emp_id from tlkp_operator / VIEW_DATAAUTH</summary>
    [Column("operator_emp_id"), MaxLength(50)]
    public string? OperatorEmpId { get; set; }

    /// <summary>Leader's emp_id from tlkp_lineGroup.lgp_leader</summary>
    [Column("leader_emp_id"), MaxLength(50)]
    public string? LeaderEmpId { get; set; }

    /// <summary>Kasubsie's emp_id from tlkp_lineGroup.lgp_kasubsie</summary>
    [Column("kasubsie_emp_id"), MaxLength(50)]
    public string? KasubsieEmpId { get; set; }

    /// <summary>Kasie's emp_id from tlkp_userKasie.kasie_emp_id</summary>
    [Column("kasie_emp_id"), MaxLength(50)]
    public string? KasieEmpId { get; set; }

    /// <summary>
    /// Battery slot assignments stored as JSON:
    /// [{"type":"NS40ZL","mold":"COS-A01"},...]
    /// </summary>
    [Column("battery_slots_json", TypeName = "text")]
    public string? BatterySlotsJson { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    [ForeignKey(nameof(FormId))]
    public CosFormDefinition? Form { get; set; }

    public ICollection<CosCheckValue> CheckValues { get; set; } = new List<CosCheckValue>();
    public ICollection<CosProblem> Problems { get; set; } = new List<CosProblem>();
    public ICollection<CosSignatureEntry> Signatures { get; set; } = new List<CosSignatureEntry>();
}

[Table("cos_check_values")]
public class CosCheckValue
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("submission_id")]
    public int SubmissionId { get; set; }

    [Required, Column("setting_key"), MaxLength(200)]
    public string SettingKey { get; set; } = null!;

    [Column("value"), MaxLength(500)]
    public string? Value { get; set; }

    [ForeignKey(nameof(SubmissionId)), JsonIgnore]
    public CosSubmission? Submission { get; set; }
}

[Table("cos_problems")]
public class CosProblem
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("submission_id")]
    public int SubmissionId { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("values_json", TypeName = "text")]
    public string? ValuesJson { get; set; }

    [ForeignKey(nameof(SubmissionId)), JsonIgnore]
    public CosSubmission? Submission { get; set; }
}

[Table("cos_signature_entries")]
public class CosSignatureEntry
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("submission_id")]
    public int SubmissionId { get; set; }

    [Required, Column("role_key"), MaxLength(50)]
    public string RoleKey { get; set; } = null!;

    [Column("signature_data", TypeName = "nvarchar(max)")]
    public string? SignatureData { get; set; }

    [ForeignKey(nameof(SubmissionId)), JsonIgnore]
    public CosSubmission? Submission { get; set; }
}

// ─── Employee Signatures (persistent per-employee) ───

[Table("cos_employee_signatures")]
public class CosEmployeeSignature
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Required, Column("emp_id"), MaxLength(50)]
    public string EmpId { get; set; } = null!;

    [Column("signature_data", TypeName = "nvarchar(max)")]
    public string? SignatureData { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
