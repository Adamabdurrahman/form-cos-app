using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

[Table("submission_check_value")]
public class SubmissionCheckValue
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    [Column("setting_key")]
    public string SettingKey { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("value")]
    public string? Value { get; set; }

    [Column("submission_id")]
    public int SubmissionId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(SubmissionId))]
    public FormSubmission? Submission { get; set; }
}
