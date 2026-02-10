using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models;

[Table("kasie")]
public class Kasie
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    // Navigation
    [JsonIgnore]
    public ICollection<Kasubsie> Kasubsies { get; set; } = new List<Kasubsie>();
}
