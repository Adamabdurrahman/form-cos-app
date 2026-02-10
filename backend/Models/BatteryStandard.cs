using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

[Table("battery_standard")]
public class BatteryStandard
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("param_key")]
    public string ParamKey { get; set; } = string.Empty;

    /// <summary>Display text, e.g. "470 - 490"</summary>
    [Required]
    [MaxLength(50)]
    [Column("value")]
    public string Value { get; set; } = string.Empty;

    /// <summary>Numeric min for validation</summary>
    [Column("min_value", TypeName = "decimal(10,2)")]
    public decimal? MinValue { get; set; }

    /// <summary>Numeric max for validation</summary>
    [Column("max_value", TypeName = "decimal(10,2)")]
    public decimal? MaxValue { get; set; }

    [Column("battery_type_id")]
    public int BatteryTypeId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(BatteryTypeId))]
    public BatteryType? BatteryType { get; set; }
}
