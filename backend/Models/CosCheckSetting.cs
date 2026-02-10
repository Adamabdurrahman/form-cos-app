using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

/// <summary>
/// Stores the value entered for each check item on a COS validation form.
/// For visual items: value is "ok", "ng", or null.
/// For numeric items: value is the measured number as string.
/// </summary>
[Table("cos_check_setting")]
public class CosCheckSetting
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// The composite key, e.g., "tempPot_1" (item key + battery slot index)
    /// or "tempMold_mold1_2" (item key + sub-row suffix + battery slot index)
    /// </summary>
    [Required]
    [MaxLength(120)]
    [Column("setting_key")]
    public string SettingKey { get; set; } = string.Empty;

    /// <summary>
    /// The value entered: "ok"/"ng" for visual, numeric string for numeric
    /// </summary>
    [MaxLength(100)]
    [Column("value")]
    public string? Value { get; set; }

    [Column("cos_validation_id")]
    public int CosValidationId { get; set; }

    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(CosValidationId))]
    public CosValidation? CosValidation { get; set; }
}
