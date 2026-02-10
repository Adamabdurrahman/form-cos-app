using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

/// <summary>
/// Defines columns for the Problem table section of a form.
/// Each form can have different problem columns.
/// </summary>
[Table("form_problem_column")]
public class FormProblemColumn
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("column_key")]
    public string ColumnKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("label")]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// "text", "number", "time"
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("field_type")]
    public string FieldType { get; set; } = "text";

    [MaxLength(20)]
    [Column("width")]
    public string? Width { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("form_id")]
    public int FormId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(FormId))]
    public FormDefinition? Form { get; set; }
}
