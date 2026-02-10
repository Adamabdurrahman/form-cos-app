using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

/// <summary>
/// Definition of each check item on the COS validation form.
/// Maps to the check items table rows.
/// </summary>
[Table("check_item")]
public class CheckItem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Unique string key, e.g., "kekuatanCastingStrap", "tempPot"
    /// </summary>
    [Required]
    [MaxLength(80)]
    [Column("item_key")]
    public string ItemKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("label")]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// "visual" or "numeric"
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Standard text for visual checks, e.g., "Ditarik tidak lepas"
    /// </summary>
    [MaxLength(200)]
    [Column("visual_standard")]
    public string? VisualStandard { get; set; }

    /// <summary>
    /// Key linking to BatteryStandard.ParamKey for numeric checks
    /// </summary>
    [MaxLength(50)]
    [Column("numeric_std_key")]
    public string? NumericStdKey { get; set; }

    /// <summary>
    /// Fixed standard override, e.g., "> 275° C"
    /// </summary>
    [MaxLength(100)]
    [Column("fixed_standard")]
    public string? FixedStandard { get; set; }

    [MaxLength(100)]
    [Column("frequency")]
    public string? Frequency { get; set; }

    [MaxLength(200)]
    [Column("keterangan")]
    public string? Keterangan { get; set; }

    [MaxLength(100)]
    [Column("conditional_label")]
    public string? ConditionalLabel { get; set; }

    /// <summary>
    /// Display order of the check item
    /// </summary>
    [Column("sort_order")]
    public int SortOrder { get; set; }

    // Navigation
    public ICollection<CheckSubRow> SubRows { get; set; } = new List<CheckSubRow>();
}
