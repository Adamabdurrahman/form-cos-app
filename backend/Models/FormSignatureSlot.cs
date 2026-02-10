using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

/// <summary>
/// Defines signature slots for a form (e.g., Dibuat, Diperiksa, Diketahui, Disetujui).
/// </summary>
[Table("form_signature_slot")]
public class FormSignatureSlot
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Key for the role, e.g. "operator", "leader", "kasubsie", "kasie"
    /// </summary>
    [Required]
    [MaxLength(30)]
    [Column("role_key")]
    public string RoleKey { get; set; } = string.Empty;

    /// <summary>
    /// Display label, e.g. "Dibuat", "Diperiksa"
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("label")]
    public string Label { get; set; } = string.Empty;

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("form_id")]
    public int FormId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(FormId))]
    public FormDefinition? Form { get; set; }
}
