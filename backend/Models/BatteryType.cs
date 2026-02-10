using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

[Table("battery_type")]
public class BatteryType
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    // Navigation
    public ICollection<BatteryMold> Molds { get; set; } = new List<BatteryMold>();
    public ICollection<BatteryStandard> Standards { get; set; } = new List<BatteryStandard>();
}
