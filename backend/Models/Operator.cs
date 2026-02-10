using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

[Table("operator")]
public class Operator
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("leader_id")]
    public int LeaderId { get; set; }

    // Navigation
    [ForeignKey(nameof(LeaderId))]
    public Leader? Leader { get; set; }
}
