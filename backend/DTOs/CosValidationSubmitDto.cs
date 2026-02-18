using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

/// <summary>
/// DTO for submitting a complete COS validation form.
/// </summary>
public class CosValidationSubmitDto
{
    [Required]
    public DateTime Tanggal { get; set; }

    [Required]
    public int Line { get; set; }

    [Required]
    public int Shift { get; set; }

    [Required]
    public int OperatorId { get; set; }

    public int? LeaderId { get; set; }
    public int? KasubsieId { get; set; }
    public int? KasieId { get; set; }

    // Battery slots (up to 3)
    public string? BatteryType1 { get; set; }
    public string? Mold1 { get; set; }
    public string? BatteryType2 { get; set; }
    public string? Mold2 { get; set; }
    public string? BatteryType3 { get; set; }
    public string? Mold3 { get; set; }

    /// <summary>
    /// Key-value pairs for all check settings.
    /// Key format: "{itemKey}_{slotIndex}" or "{itemKey}_{suffix}_{slotIndex}"
    /// Value: "ok"/"ng" for visual, numeric string for numeric
    /// </summary>
    public Dictionary<string, string?> Settings { get; set; } = new();

    /// <summary>
    /// Problem/issue rows
    /// </summary>
    public List<CosProblemDto> Problems { get; set; } = new();

    /// <summary>
    /// Signature data per role
    /// </summary>
    public Dictionary<string, string?> Signatures { get; set; } = new();
}

public class CosProblemDto
{
    public string? Problem { get; set; }
    public string? Action { get; set; }
}
