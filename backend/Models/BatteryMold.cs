using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

[Table("battery_mold")]
public class BatteryMold
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("battery_type_id")]
    public int BatteryTypeId { get; set; }

    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(BatteryTypeId))]
    public BatteryType? BatteryType { get; set; }
}
