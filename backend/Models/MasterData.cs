using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

// ══════════════════════════════════════════════════
// MASTER DATA TABLES (from db_master_data / script.sql)
// These tables mirror the existing SQL Server structure.
// EF Core migration will create them in MySQL.
// ══════════════════════════════════════════════════

[Table("tlkp_plant")]
public class TlkpPlant
{
    [Key, Column("plant_id")]
    public int PlantId { get; set; }

    [Column("plant_code"), MaxLength(10)]
    public string? PlantCode { get; set; }

    [Column("plant_name"), MaxLength(50)]
    public string? PlantName { get; set; }

    [Column("plant_status")]
    public int? PlantStatus { get; set; }

    [Column("create_date")]
    public DateTime? CreateDate { get; set; }

    [Column("create_by"), MaxLength(50)]
    public string? CreateBy { get; set; }

    [Column("modify_date")]
    public DateTime? ModifyDate { get; set; }

    [Column("modify_by"), MaxLength(50)]
    public string? ModifyBy { get; set; }
}

[Table("tlkp_divisi")]
public class TlkpDivisi
{
    [Key, Column("div_id")]
    public int DivId { get; set; }

    [Column("div_name"), MaxLength(100)]
    public string? DivName { get; set; }

    [Column("div_status")]
    public int? DivStatus { get; set; }

    [Column("plant_id")]
    public int? PlantId { get; set; }

    [Column("div_head"), MaxLength(50)]
    public string? DivHead { get; set; }

    [Column("div_kode"), MaxLength(50)]
    public string? DivKode { get; set; }

    [Column("div_initial"), MaxLength(50)]
    public string? DivInitial { get; set; }

    [Column("create_date")]
    public DateTime? CreateDate { get; set; }

    [Column("create_by"), MaxLength(50)]
    public string? CreateBy { get; set; }

    [Column("modify_date")]
    public DateTime? ModifyDate { get; set; }

    [Column("modify_by"), MaxLength(50)]
    public string? ModifyBy { get; set; }
}

[Table("tlkp_departemen")]
public class TlkpDepartemen
{
    [Key, Column("dep_id")]
    public int DepId { get; set; }

    [Column("dep_name"), MaxLength(100)]
    public string? DepName { get; set; }

    [Column("dep_status")]
    public int? DepStatus { get; set; }

    [Column("plant_id")]
    public int? PlantId { get; set; }

    [Column("dep_head"), MaxLength(50)]
    public string? DepHead { get; set; }

    [Column("dep_kode"), MaxLength(50)]
    public string? DepKode { get; set; }

    [Column("div_id"), MaxLength(50)]
    public string? DivId { get; set; }

    [Column("dep_initial"), MaxLength(50)]
    public string? DepInitial { get; set; }

    [Column("create_date")]
    public DateTime? CreateDate { get; set; }

    [Column("create_by"), MaxLength(50)]
    public string? CreateBy { get; set; }

    [Column("modify_date")]
    public DateTime? ModifyDate { get; set; }

    [Column("modify_by"), MaxLength(50)]
    public string? ModifyBy { get; set; }
}

[Table("tlkp_section")]
public class TlkpSection
{
    [Key, Column("sec_id")]
    public int SecId { get; set; }

    [Column("sec_name"), MaxLength(50)]
    public string? SecName { get; set; }

    [Column("dep_id")]
    public int? DepId { get; set; }

    [Column("sec_status")]
    public int? SecStatus { get; set; }

    [Column("sec_head"), MaxLength(50)]
    public string? SecHead { get; set; }

    [Column("sec_kode"), MaxLength(50)]
    public string? SecKode { get; set; }

    [Column("sec_plant")]
    public int? SecPlant { get; set; }

    [Column("create_date")]
    public DateTime? CreateDate { get; set; }

    [Column("create_by"), MaxLength(50)]
    public string? CreateBy { get; set; }

    [Column("modify_date")]
    public DateTime? ModifyDate { get; set; }

    [Column("modify_by"), MaxLength(50)]
    public string? ModifyBy { get; set; }
}

[Table("tlkp_line")]
public class TlkpLine
{
    [Key, Column("line_id")]
    public int LineId { get; set; }

    [Column("line_name"), MaxLength(100)]
    public string? LineName { get; set; }

    [Column("line_status")]
    public int? LineStatus { get; set; }

    [Column("sec_id")]
    public int? SecId { get; set; }

    [Column("crea_date")]
    public DateTime? CreaDate { get; set; }

    [Column("crea_by"), MaxLength(50)]
    public string? CreaBy { get; set; }

    [Column("modi_by"), MaxLength(50)]
    public string? ModiBy { get; set; }

    [Column("modi_date")]
    public DateTime? ModiDate { get; set; }

    [Column("work_center"), MaxLength(50)]
    public string? WorkCenter { get; set; }

    [Column("line_ket"), MaxLength(50)]
    public string? LineKet { get; set; }

    [Column("line_plant")]
    public int? LinePlant { get; set; }

    // NOTE: Kolom userKasie_id tidak ada di DBPORTAL (production).
    // Uncomment jika pakai GSPORTAL-DEV01.
    // [Column("userKasie_id")]
    // public int? UserKasieId { get; set; }
}

[Table("tlkp_group")]
public class TlkpGroup
{
    [Key, Column("group_id")]
    public int GroupId { get; set; }

    [Column("group_name"), MaxLength(100)]
    public string? GroupName { get; set; }

    [Column("group_status")]
    public int? GroupStatus { get; set; }

    [Column("create_date")]
    public DateTime? CreateDate { get; set; }

    [Column("create_by"), MaxLength(50)]
    public string? CreateBy { get; set; }

    [Column("modify_date")]
    public DateTime? ModifyDate { get; set; }

    [Column("modify_by"), MaxLength(50)]
    public string? ModifyBy { get; set; }
}

[Table("tlkp_shift")]
public class TlkpShift
{
    [Key, Column("shift_id")]
    public int ShiftId { get; set; }

    [Column("shift_name"), MaxLength(100)]
    public string? ShiftName { get; set; }

    [Column("shift_start")]
    public TimeSpan? ShiftStart { get; set; }

    [Column("shift_end")]
    public TimeSpan? ShiftEnd { get; set; }

    [Column("shift_status")]
    public int? ShiftStatus { get; set; }

    [Column("shift_plan"), MaxLength(50)]
    public string? ShiftPlan { get; set; }

    [Column("shift_code"), MaxLength(50)]
    public string? ShiftCode { get; set; }
}

/// <summary>
/// Line Group — contains leader (emp_id) and kasubsie (emp_id).
/// This is the key organizational unit linking line, group, leader, kasubsie, and kasie.
/// </summary>
[Table("tlkp_lineGroup")]
public class TlkpLineGroup
{
    [Key, Column("lgp_id")]
    public int LgpId { get; set; }

    [Column("line_id")]
    public int? LineId { get; set; }

    [Column("group_id")]
    public int? GroupId { get; set; }

    [Column("lgp_status")]
    public int? LgpStatus { get; set; }

    /// <summary>Leader's emp_id (join to VIEW_DATAAUTH for name)</summary>
    [Column("lgp_leader"), MaxLength(50)]
    public string? LgpLeader { get; set; }

    /// <summary>Kasubsie's emp_id (join to VIEW_DATAAUTH for name)</summary>
    [Column("lgp_kasubsie"), MaxLength(50)]
    public string? LgpKasubsie { get; set; }

    [Column("lgp_plant")]
    public int? LgpPlant { get; set; }

    [Column("create_date")]
    public DateTime? CreateDate { get; set; }

    [Column("create_by"), MaxLength(50)]
    public string? CreateBy { get; set; }

    [Column("modify_date")]
    public DateTime? ModifyDate { get; set; }

    [Column("modify_by"), MaxLength(50)]
    public string? ModifyBy { get; set; }

    [Column("userKasie_id")]
    public int? UserKasieId { get; set; }
}

/// <summary>
/// Operator — composite PK (user_id + jopr_id).
/// user_id = employee ID / NPK.
/// </summary>
[Table("tlkp_operator")]
public class TlkpOperator
{
    [Column("user_id"), MaxLength(50)]
    public string UserId { get; set; } = null!;

    [Column("lgp_id")]
    public int? LgpId { get; set; }

    [Column("jopr_id")]
    public int JoprId { get; set; }

    [Column("klasifikasi"), MaxLength(50)]
    public string? Klasifikasi { get; set; }

    [Column("group_id")]
    public int? GroupId { get; set; }

    [Column("userKasie_id")]
    public int? UserKasieId { get; set; }

    [Column("update_date")]
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// Kasie assignment: links a section to a kasie employee.
/// </summary>
[Table("tlkp_userKasie")]
public class TlkpUserKasie
{
    [Key, Column("userKasie_id")]
    public int UserKasieId { get; set; }

    [Column("sec_id")]
    public int SecId { get; set; }

    [Column("kasie_emp_id"), MaxLength(20)]
    public string KasieEmpId { get; set; } = null!;

    [Column("create_date")]
    public DateTime? CreateDate { get; set; }
}

[Table("tlkp_user")]
public class TlkpUser
{
    [Key, Column("user_npk"), MaxLength(10)]
    public string UserNpk { get; set; } = null!;

    [Column("user_nama"), MaxLength(100)]
    public string? UserNama { get; set; }

    [Column("user_namafull"), MaxLength(100)]
    public string? UserNamaFull { get; set; }

    [Column("user_pass"), MaxLength(50)]
    public string? UserPass { get; set; }

    [Column("user_email"), MaxLength(50)]
    public string? UserEmail { get; set; }

    [Column("user_role"), MaxLength(50)]
    public string? UserRole { get; set; }

    [Column("user_lastLogin")]
    public DateTime? UserLastLogin { get; set; }

    [Column("user_createBy"), MaxLength(10)]
    public string? UserCreateBy { get; set; }

    [Column("user_modifBy"), MaxLength(10)]
    public string? UserModifBy { get; set; }

    [Column("user_createDate")]
    public DateTime? UserCreateDate { get; set; }

    [Column("user_modifDate")]
    public DateTime? UserModifDate { get; set; }

    [Column("user_status")]
    public int? UserStatus { get; set; }

    [Column("emp_id"), MaxLength(50)]
    public string? EmpId { get; set; }

    [Column("dept_id"), MaxLength(50)]
    public string? DeptId { get; set; }

    [Column("user_plant")]
    public int? UserPlant { get; set; }
}

[Table("tlkp_item")]
public class TlkpItem
{
    [Column("item_num"), MaxLength(100)]
    public string ItemNum { get; set; } = null!;

    [Column("kat_id")]
    public int KatId { get; set; }

    [Column("item_status")]
    public int? ItemStatus { get; set; }

    [Column("item_desc"), MaxLength(500)]
    public string? ItemDesc { get; set; }

    [Column("series_id")]
    public int? SeriesId { get; set; }

    [Column("item_na"), MaxLength(10)]
    public string? ItemNa { get; set; }

    [Column("item_asam")]
    public int? ItemAsam { get; set; }

    [Column("item_ds"), MaxLength(10)]
    public string? ItemDs { get; set; }

    [Column("item_lot")]
    public int? ItemLot { get; set; }

    [Column("item_keterangan"), MaxLength(50)]
    public string? ItemKeterangan { get; set; }

    [Column("item_tgl")]
    public DateTime? ItemTgl { get; set; }

    [Column("creadate")]
    public DateTime? CreaDate { get; set; }

    [Column("creaby"), MaxLength(50)]
    public string? CreaBy { get; set; }

    [Column("modidate")]
    public DateTime? ModiDate { get; set; }

    [Column("modiby"), MaxLength(50)]
    public string? ModiBy { get; set; }
}

[Table("tlkp_kategori")]
public class TlkpKategori
{
    [Key, Column("kat_id")]
    public int KatId { get; set; }

    [Column("kat_name"), MaxLength(100)]
    public string? KatName { get; set; }

    [Column("kat_status")]
    public int? KatStatus { get; set; }

    [Column("item_pch"), MaxLength(50)]
    public string? ItemPch { get; set; }

    [Column("kat_plant")]
    public int? KatPlant { get; set; }

    [Column("create_date")]
    public DateTime? CreateDate { get; set; }

    [Column("create_by"), MaxLength(50)]
    public string? CreateBy { get; set; }

    [Column("modify_date")]
    public DateTime? ModifyDate { get; set; }

    [Column("modify_by"), MaxLength(50)]
    public string? ModifyBy { get; set; }
}

[Table("tlkp_series")]
public class TlkpSeries
{
    [Key, Column("series_id")]
    public int SeriesId { get; set; }

    [Column("series_name"), MaxLength(10)]
    public string? SeriesName { get; set; }

    [Column("created_by"), MaxLength(50)]
    public string? CreatedBy { get; set; }

    [Column("created_date")]
    public DateTime? CreatedDate { get; set; }

    [Column("modif_by"), MaxLength(50)]
    public string? ModifBy { get; set; }

    [Column("modif_date")]
    public DateTime? ModifDate { get; set; }

    [Column("series_status")]
    public int? SeriesStatus { get; set; }

    [Column("series_plant")]
    public int? SeriesPlant { get; set; }
}

[Table("tlkp_mold")]
public class TlkpMold
{
    [Key, Column("mold_code"), MaxLength(50)]
    public string MoldCode { get; set; } = null!;

    [Column("mold_description"), MaxLength(100)]
    public string? MoldDescription { get; set; }

    [Column("item_num"), MaxLength(100)]
    public string? ItemNum { get; set; }

    [Column("mold_sts"), MaxLength(50)]
    public string? MoldSts { get; set; }

    [Column("created_by"), MaxLength(50)]
    public string? CreatedBy { get; set; }

    [Column("created_date")]
    public DateTime? CreatedDate { get; set; }

    [Column("modif_by"), MaxLength(50)]
    public string? ModifBy { get; set; }

    [Column("modif_date")]
    public DateTime? ModifDate { get; set; }

    [Column("mold_status"), MaxLength(50)]
    public string? MoldStatus { get; set; }

    [Column("id_section")]
    public int? IdSection { get; set; }
}

[Table("tlkp_moldtype")]
public class TlkpMoldType
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("mt_name"), MaxLength(50)]
    public string? MtName { get; set; }

    [Column("mt_status")]
    public int? MtStatus { get; set; }

    [Column("mt_createBy"), MaxLength(10)]
    public string? MtCreateBy { get; set; }

    [Column("mt_createDate")]
    public DateTime? MtCreateDate { get; set; }

    [Column("mt_modifBy"), MaxLength(10)]
    public string? MtModifBy { get; set; }

    [Column("mt_modifDate")]
    public DateTime? MtModifDate { get; set; }

    [Column("mt_plant")]
    public int? MtPlant { get; set; }
}

[Table("tlkp_job")]
public class TlkpJob
{
    [Key, Column("jopr_id")]
    public int JoprId { get; set; }

    [Column("jopr_name"), MaxLength(100)]
    public string? JoprName { get; set; }

    [Column("jopr_status")]
    public int? JoprStatus { get; set; }

    [Column("sec_id")]
    public int? SecId { get; set; }

    [Column("jopr_alias"), MaxLength(50)]
    public string? JoprAlias { get; set; }

    [Column("create_date")]
    public DateTime? CreateDate { get; set; }

    [Column("create_by"), MaxLength(50)]
    public string? CreateBy { get; set; }

    [Column("modify_date")]
    public DateTime? ModifyDate { get; set; }

    [Column("modify_by"), MaxLength(50)]
    public string? ModifyBy { get; set; }

    // NOTE: Kolom userKasie_id tidak ada di DBPORTAL (production).
    // Uncomment jika pakai GSPORTAL-DEV01.
    // [Column("userKasie_id")]
    // public int? UserKasieId { get; set; }
}

/// <summary>
/// Employee data (synced from Sunfish HR system).
/// FULL SCHEMA — matches [dbo].[VIEW_DATAAUTH] on SQL Server exactly.
/// </summary>
[Table("VIEW_DATAAUTH")]
public class ViewDataAuth
{
    [Key, Column("id_recnum")]
    public long IdRecnum { get; set; }

    // ═══ Personal Info ═══
    [Column("emp_id"), MaxLength(50)]
    public string? EmpId { get; set; }

    [Column("first_name"), MaxLength(50)]
    public string? FirstName { get; set; }

    [Column("middle_name"), MaxLength(50)]
    public string? MiddleName { get; set; }

    [Column("last_name"), MaxLength(50)]
    public string? LastName { get; set; }

    [Column("Full_Name"), MaxLength(302)]
    public string? FullName { get; set; }

    [Column("gender")]
    public byte? Gender { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("taxno"), MaxLength(50)]
    public string? Taxno { get; set; }

    [Column("geocoord"), MaxLength(50)]
    public string? Geocoord { get; set; }

    [Column("status")]
    public byte? Status { get; set; }

    [Column("req_status")]
    public byte? ReqStatus { get; set; }

    [Column("lastreqno"), MaxLength(50)]
    public string? Lastreqno { get; set; }

    [Column("email"), MaxLength(100)]
    public string? Email { get; set; }

    [Column("photo"), MaxLength(100)]
    public string? Photo { get; set; }

    [Column("phone"), MaxLength(255)]
    public string? Phone { get; set; }

    [Column("birthplace"), MaxLength(50)]
    public string? Birthplace { get; set; }

    [Column("birthdate")]
    public DateTime? Birthdate { get; set; }

    [Column("maritalstatus")]
    public byte? Maritalstatus { get; set; }

    [Column("address"), MaxLength(255)]
    public string? Address { get; set; }

    [Column("city_id")]
    public int? CityId { get; set; }

    [Column("state_id")]
    public int? StateId { get; set; }

    [Column("country_id")]
    public int? CountryId { get; set; }

    [Column("zipcode"), MaxLength(50)]
    public string? Zipcode { get; set; }

    // ═══ Employment Info ═══
    [Column("company_id")]
    public int? CompanyId { get; set; }

    [Column("emp_no"), MaxLength(50)]
    public string? EmpNo { get; set; }

    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    [Column("is_main")]
    public byte? IsMain { get; set; }

    [Column("empcompany_status")]
    public byte? EmpcompanyStatus { get; set; }

    [Column("grade_code"), MaxLength(50)]
    public string? GradeCode { get; set; }

    [Column("employ_code"), MaxLength(50)]
    public string? EmployCode { get; set; }

    [Column("cost_code"), MaxLength(50)]
    public string? CostCode { get; set; }

    // ═══ Supervisor Hierarchy ═══
    [Column("spv_parent"), MaxLength(50)]
    public string? SpvParent { get; set; }

    [Column("spv_pos")]
    public int? SpvPos { get; set; }

    [Column("spv_path"), MaxLength(8000)]
    public string? SpvPath { get; set; }

    [Column("spv_level")]
    public byte? SpvLevel { get; set; }

    // ═══ Manager Hierarchy ═══
    [Column("mgr_parent"), MaxLength(50)]
    public string? MgrParent { get; set; }

    [Column("mgr_pos")]
    public int? MgrPos { get; set; }

    [Column("mgr_path"), MaxLength(8000)]
    public string? MgrPath { get; set; }

    [Column("mgr_level")]
    public byte? MgrLevel { get; set; }

    // ═══ Position Info ═══
    [Column("position_id")]
    public int? PositionId { get; set; }

    [Column("pos_code"), MaxLength(50)]
    public string? PosCode { get; set; }

    [Column("parent_id")]
    public int? ParentId { get; set; }

    [Column("pos_name_en"), MaxLength(100)]
    public string? PosNameEn { get; set; }

    [Column("pos_name_id"), MaxLength(100)]
    public string? PosNameId { get; set; }

    [Column("pos_name_my"), MaxLength(100)]
    public string? PosNameMy { get; set; }

    [Column("pos_name_th"), MaxLength(100)]
    public string? PosNameTh { get; set; }

    [Column("jobstatuscode"), MaxLength(100)]
    public string? Jobstatuscode { get; set; }

    [Column("pos_level")]
    public int? PosLevel { get; set; }

    [Column("parent_path"), MaxLength(255)]
    public string? ParentPath { get; set; }

    [Column("pos_flag")]
    public byte? PosFlag { get; set; }

    // ═══ Department Info ═══
    [Column("dept_id")]
    public int? DeptId { get; set; }

    [Column("dorder")]
    public int? Dorder { get; set; }

    [Column("jobtitle_code"), MaxLength(50)]
    public string? JobtitleCode { get; set; }

    [Column("report_topos")]
    public int? ReportTopos { get; set; }

    [Column("clevel")]
    public int? Clevel { get; set; }

    [Column("corder")]
    public int? Corder { get; set; }

    [Column("changeflag"), MaxLength(3)]
    public string? Changeflag { get; set; }

    [Column("report_postype"), MaxLength(2)]
    public string? ReportPostype { get; set; }

    [Column("dept_code"), MaxLength(50)]
    public string? DeptCode { get; set; }

    [Column("grade_order")]
    public int? GradeOrder { get; set; }

    [Column("grade_category"), MaxLength(255)]
    public string? GradeCategory { get; set; }

    [Column("worklocation_code"), MaxLength(50)]
    public string? WorklocationCode { get; set; }
}

/// <summary>
/// Simplified employee view (synced from Sunfish).
/// Keyless entity — read only.
/// </summary>
[Table("VIEW_EMPLOYEE")]
public class ViewEmployee
{
    [Column("emp_id")]
    public string? EmpId { get; set; }

    [Column("plant")]
    public string? Plant { get; set; }

    [Column("npk")]
    public string? Npk { get; set; }

    [Column("dep_id")]
    public long? DepId { get; set; }

    [Column("dep")]
    public string? Dep { get; set; }

    [Column("sec_id")]
    public long? SecId { get; set; }

    [Column("sec")]
    public string? Sec { get; set; }
}
