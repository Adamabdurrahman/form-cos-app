using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

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

    [MaxLength(100)]
    [Column("fixed_standard")]
    public string? FixedStandard { get; set; }

    [Column("fixed_min", TypeName = "decimal(10,2)")]
    public decimal? FixedMin { get; set; }

    [Column("fixed_max", TypeName = "decimal(10,2)")]
    public decimal? FixedMax { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("check_item_id")]
    public int CheckItemId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(CheckItemId))]
    public CheckItem? CheckItem { get; set; }
}
