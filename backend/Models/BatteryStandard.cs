using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

/// <summary>
/// Stores the standard value range per battery type per parameter key.
/// Key examples: pourWait, pourTime, tempPot, tempMold, etc.
/// </summary>
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

    [Required]
    [MaxLength(50)]
    [Column("value")]
    public string Value { get; set; } = string.Empty;

    [Column("battery_type_id")]
    public int BatteryTypeId { get; set; }

    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(BatteryTypeId))]
    public BatteryType? BatteryType { get; set; }
}
