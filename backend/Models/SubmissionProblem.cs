using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

/// <summary>
/// Problem row in a submitted form.
/// Values stored as JSON to support dynamic columns per form.
/// </summary>
[Table("submission_problem")]
public class SubmissionProblem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// JSON object with column_key → value pairs.
    /// e.g. {"item":"Temp Pot","masalah":"Terlalu panas","tindakan":"Turunkan suhu"}
    /// </summary>
    [Column("values_json")]
    public string? ValuesJson { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("submission_id")]
    public int SubmissionId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(SubmissionId))]
    public FormSubmission? Submission { get; set; }
}
