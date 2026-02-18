using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

/// <summary>
/// Tabel t_cos_user_accounts di db_cos_checksheet.
/// Menyimpan kredensial login (NPK ↔ Username/Password).
/// Data karyawan tetap dirujuk dari db_master_data.VIEW_EMPLOYEE via NPK.
/// </summary>
[Table("t_cos_user_accounts")]
public class UserAccount
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("npk")]
    [MaxLength(20)]
    public string Npk { get; set; } = string.Empty;

    [Required]
    [Column("username")]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [Column("password_hash")]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
