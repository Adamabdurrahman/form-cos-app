using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

/// <summary>
/// Definition of each check item on a form.
/// </summary>
[Table("check_item")]
public class CheckItem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("form_id")]
    public int FormId { get; set; }

    [Required]
    [MaxLength(80)]
    [Column("item_key")]
    public string ItemKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("label")]
    public string Label { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("type")]
    public string Type { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("visual_standard")]
    public string? VisualStandard { get; set; }

    [MaxLength(50)]
    [Column("numeric_std_key")]
    public string? NumericStdKey { get; set; }

    [MaxLength(100)]
    [Column("fixed_standard")]
    public string? FixedStandard { get; set; }

    /// <summary>Numeric min for fixed standard validation</summary>
    [Column("fixed_min", TypeName = "decimal(10,2)")]
    public decimal? FixedMin { get; set; }

    /// <summary>Numeric max for fixed standard validation</summary>
    [Column("fixed_max", TypeName = "decimal(10,2)")]
    public decimal? FixedMax { get; set; }

    [MaxLength(100)]
    [Column("frequency")]
    public string? Frequency { get; set; }

    [MaxLength(200)]
    [Column("keterangan")]
    public string? Keterangan { get; set; }

    [MaxLength(100)]
    [Column("conditional_label")]
    public string? ConditionalLabel { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }

    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(FormId))]
    public FormDefinition? Form { get; set; }

    public ICollection<CheckSubRow> SubRows { get; set; } = new List<CheckSubRow>();
}
