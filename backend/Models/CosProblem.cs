using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

/// <summary>
/// Problem/issue row in the COS validation form.
/// </summary>
[Table("cos_problem")]
public class CosProblem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [MaxLength(500)]
    [Column("problem")]
    public string? Problem { get; set; }

    [MaxLength(500)]
    [Column("action")]
    public string? Action { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("cos_validation_id")]
    public int CosValidationId { get; set; }

    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(CosValidationId))]
    public CosValidation? CosValidation { get; set; }
}
