using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

[Table("kasubsie")]
public class Kasubsie
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("kasie_id")]
    public int KasieId { get; set; }

    // Navigation
    [ForeignKey(nameof(KasieId))]
    public Kasie? Kasie { get; set; }

    [JsonIgnore]
    public ICollection<Leader> Leaders { get; set; } = new List<Leader>();
}
