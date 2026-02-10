using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

[Table("leader")]
public class Leader
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("kasubsie_id")]
    public int KasubsieId { get; set; }

    // Navigation
    [ForeignKey(nameof(KasubsieId))]
    public Kasubsie? Kasubsie { get; set; }

    [JsonIgnore]
    public ICollection<Operator> Operators { get; set; } = new List<Operator>();
}
