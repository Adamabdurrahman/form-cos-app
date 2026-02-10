using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

[Table("submission_signature")]
public class SubmissionSignature
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    [Column("role_key")]
    public string RoleKey { get; set; } = string.Empty;

    [Column("signature_data")]
    public string? SignatureData { get; set; }

    [Column("submission_id")]
    public int SubmissionId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(SubmissionId))]
    public FormSubmission? Submission { get; set; }
}
