using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

/// <summary>
/// A submitted form record (generalized from CosValidation).
/// </summary>
[Table("form_submission")]
public class FormSubmission
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("form_id")]
    public int FormId { get; set; }

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
    /// Battery slot assignments stored as JSON:
    /// [{"type":"NS40ZL","mold":"COS-A01"},{"type":"NS60L","mold":"COS-B01"},...]
    /// </summary>
    [Column("battery_slots_json")]
    public string? BatterySlotsJson { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    [ForeignKey(nameof(FormId))]
    public FormDefinition? Form { get; set; }

    [ForeignKey(nameof(OperatorId))]
    public Operator? Operator { get; set; }

    [ForeignKey(nameof(LeaderId))]
    public Leader? Leader { get; set; }

    [ForeignKey(nameof(KasubsieId))]
    public Kasubsie? Kasubsie { get; set; }

    [ForeignKey(nameof(KasieId))]
    public Kasie? Kasie { get; set; }

    public ICollection<SubmissionCheckValue> CheckValues { get; set; } = new List<SubmissionCheckValue>();
    public ICollection<SubmissionProblem> Problems { get; set; } = new List<SubmissionProblem>();
    public ICollection<SubmissionSignature> Signatures { get; set; } = new List<SubmissionSignature>();
}
