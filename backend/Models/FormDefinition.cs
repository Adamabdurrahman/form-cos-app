using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

/// <summary>
/// Master definition of a form (e.g. COS Validation, Formation Check, etc.)
/// </summary>
[Table("form_definition")]
public class FormDefinition
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("subtitle")]
    public string? Subtitle { get; set; }

    /// <summary>
    /// Number of battery/product slots in the check table (default 3)
    /// </summary>
    [Column("slot_count")]
    public int SlotCount { get; set; } = 3;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<CheckItem> CheckItems { get; set; } = new List<CheckItem>();
    public ICollection<FormProblemColumn> ProblemColumns { get; set; } = new List<FormProblemColumn>();
    public ICollection<FormSignatureSlot> SignatureSlots { get; set; } = new List<FormSignatureSlot>();
    public ICollection<FormSubmission> Submissions { get; set; } = new List<FormSubmission>();
}
