using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

/// <summary>
/// A submitted COS validation form record.
/// </summary>
[Table("cos_validation")]
public class CosValidation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("tanggal")]
    public DateTime Tanggal { get; set; }

    [Column("line")]
    public int Line { get; set; }

    [Column("shift")]
    public int Shift { get; set; }

    [Column("operator_id")]
    public int OperatorId { get; set; }

    [Column("leader_id")]
    public int? LeaderId { get; set; }

    [Column("kasubsie_id")]
    public int? KasubsieId { get; set; }

    [Column("kasie_id")]
    public int? KasieId { get; set; }

    /// <summary>
    /// Battery type & mold for slot 1
    /// </summary>
    [MaxLength(100)]
    [Column("battery_type_1")]
    public string? BatteryType1 { get; set; }

    [MaxLength(50)]
    [Column("mold_1")]
    public string? Mold1 { get; set; }

    [MaxLength(100)]
    [Column("battery_type_2")]
    public string? BatteryType2 { get; set; }

    [MaxLength(50)]
    [Column("mold_2")]
    public string? Mold2 { get; set; }

    [MaxLength(100)]
    [Column("battery_type_3")]
    public string? BatteryType3 { get; set; }

    [MaxLength(50)]
    [Column("mold_3")]
    public string? Mold3 { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    [ForeignKey(nameof(OperatorId))]
    public Operator? Operator { get; set; }

    [ForeignKey(nameof(LeaderId))]
    public Leader? Leader { get; set; }

    [ForeignKey(nameof(KasubsieId))]
    public Kasubsie? Kasubsie { get; set; }

    [ForeignKey(nameof(KasieId))]
    public Kasie? Kasie { get; set; }

    public ICollection<CosCheckSetting> CheckSettings { get; set; } = new List<CosCheckSetting>();
    public ICollection<CosProblem> Problems { get; set; } = new List<CosProblem>();
    public ICollection<CosSignature> Signatures { get; set; } = new List<CosSignature>();
}
