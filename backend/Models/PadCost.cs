using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

// ══════════════════════════════════════════════════
// t_achievement (Parent)
// ══════════════════════════════════════════════════

[Table("t_achievement")]
public class Achievement
{
    [Key]
    [Column("achi_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AchiId { get; set; }

    [Column("achi_date")]
    public DateTime? AchiDate { get; set; }

    [Column("shift_id")]
    public int? ShiftId { get; set; }

    [Column("grup_id")]
    public int? GrupId { get; set; }

    [Column("achi_indirectLabor", TypeName = "decimal(18,2)")]
    public decimal? AchiIndirectLabor { get; set; }

    [Column("achi_indirectLabor_ot", TypeName = "decimal(18,2)")]
    public decimal? AchiIndirectLaborOt { get; set; }

    [Column("achi_kasubsie")]
    [MaxLength(50)]
    public string? AchiKasubsie { get; set; }

    [Column("achi_leader")]
    [MaxLength(50)]
    public string? AchiLeader { get; set; }

    [Column("id_section")]
    [MaxLength(50)]
    public string? IdSection { get; set; }

    [Column("temp_line")]
    [MaxLength(50)]
    public string? TempLine { get; set; }

    [Column("start_shift")]
    public DateTime? StartShift { get; set; }

    [Column("end_shift")]
    public DateTime? EndShift { get; set; }

    // ── Navigation Properties (One-to-Many) ──
    public virtual ICollection<ActAchievement> ActAchievements { get; set; } = new List<ActAchievement>();
    public virtual ICollection<Absensi> Absensis { get; set; } = new List<Absensi>();
}

// ══════════════════════════════════════════════════
// t_actAchievement (Child of t_achievement)
// ══════════════════════════════════════════════════

[Table("t_actAchievement")]
public class ActAchievement
{
    [Key]
    [Column("actl_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ActlId { get; set; }

    [Column("actl_item")]
    [MaxLength(50)]
    public string? ActlItem { get; set; }

    [Column("actl_planQty", TypeName = "decimal(18,2)")]
    public decimal? ActlPlanQty { get; set; }

    [Column("actl_fgQty", TypeName = "decimal(18,2)")]
    public decimal? ActlFgQty { get; set; }

    [Column("achi_id")]
    public int? AchiId { get; set; }

    [Column("line_id")]
    public int? LineId { get; set; }

    [Column("app_kasubsi")]
    [MaxLength(50)]
    public string? AppKasubsi { get; set; }

    [Column("app_kasupsi_tgl")]
    public DateTime? AppKasubsiTgl { get; set; }

    [Column("app_kasubsi_catatan")]
    [MaxLength(100)]
    public string? AppKasubsiCatatan { get; set; }

    [Column("app_req_tgl")]
    public DateTime? AppReqTgl { get; set; }

    [Column("actl_flag")]
    [MaxLength(50)]
    public string? ActlFlag { get; set; }

    [Column("crea_by")]
    [MaxLength(50)]
    public string? CreaBy { get; set; }

    [Column("crea_date")]
    public DateTime? CreaDate { get; set; }

    [Column("app_kasie")]
    public short? AppKasie { get; set; }

    [Column("app_kasie_upd_date")]
    public DateTime? AppKasieUpdDate { get; set; }

    [Column("kss_revision_reason")]
    [MaxLength(160)]
    public string? KssRevisionReason { get; set; }

    [Column("app_reject_reason")]
    [MaxLength(100)]
    public string? AppRejectReason { get; set; }

    [Column("mold_code")]
    [MaxLength(50)]
    public string? MoldCode { get; set; }

    [Column("mesin_nomor")]
    [MaxLength(50)]
    public string? MesinNomor { get; set; }

    [Column("actl_planQtyWo", TypeName = "decimal(18,2)")]
    public decimal? ActlPlanQtyWo { get; set; }

    // ── Navigation Property (Many-to-One) ──
    [ForeignKey("AchiId")]
    public virtual Achievement? Achievement { get; set; }
}

// ══════════════════════════════════════════════════
// t_absensi (Child of t_achievement — Composite PK)
// ══════════════════════════════════════════════════

[Table("t_absensi")]
public class Absensi
{
    [Column("achi_id")]
    public int AchiId { get; set; }

    [Column("abse_npk")]
    [MaxLength(50)]
    public string AbseNpk { get; set; } = null!;

    [Column("abse_cat")]
    [MaxLength(50)]
    public string? AbseCat { get; set; }

    [Column("abse_ot")]
    [MaxLength(50)]
    public string? AbseOt { get; set; }

    [Column("abse_otHours", TypeName = "decimal(18,1)")]
    public decimal? AbseOtHours { get; set; }

    [Column("abse_status")]
    public int? AbseStatus { get; set; }

    // ── Navigation Property (Many-to-One) ──
    [ForeignKey("AchiId")]
    public virtual Achievement? Achievement { get; set; }
}
