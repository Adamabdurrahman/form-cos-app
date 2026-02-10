using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

/// <summary>
/// Sub-rows for check items with multiple measurements
/// (e.g., +/- for CastingStrap, L/R for Pipe, Mold1/Mold2 for Mold Temp)
/// </summary>
[Table("check_sub_row")]
public class CheckSubRow
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    [Column("suffix")]
    public string Suffix { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("label")]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Optional fixed standard for this specific sub-row
    /// </summary>
    [MaxLength(100)]
    [Column("fixed_standard")]
    public string? FixedStandard { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("check_item_id")]
    public int CheckItemId { get; set; }

    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(CheckItemId))]
    public CheckItem? CheckItem { get; set; }
}
