using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

/// <summary>
/// Stores signature data (base64) for each signer role on a COS validation form.
/// </summary>
[Table("cos_signature")]
public class CosSignature
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Role key: "operator", "leader", "kasubsie", "kasie"
    /// </summary>
    [Required]
    [MaxLength(30)]
    [Column("role")]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Base64-encoded signature image (data:image/png;base64,...)
    /// </summary>
    [Column("signature_data")]
    public string? SignatureData { get; set; }

    [Column("cos_validation_id")]
    public int CosValidationId { get; set; }

    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(CosValidationId))]
    public CosValidation? CosValidation { get; set; }
}
