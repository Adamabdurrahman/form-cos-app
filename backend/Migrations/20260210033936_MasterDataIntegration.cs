using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class MasterDataIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_battery_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kat_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_battery_types", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_form_definitions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    subtitle = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    doc_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    revision = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    effective_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    slot_count = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_form_definitions", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_departemen",
                columns: table => new
                {
                    dep_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    dep_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dep_status = table.Column<int>(type: "int", nullable: true),
                    plant_id = table.Column<int>(type: "int", nullable: true),
                    dep_head = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dep_kode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    div_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dep_initial = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    create_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modify_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modify_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_departemen", x => x.dep_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_divisi",
                columns: table => new
                {
                    div_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    div_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    div_status = table.Column<int>(type: "int", nullable: true),
                    plant_id = table.Column<int>(type: "int", nullable: true),
                    div_head = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    div_kode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    div_initial = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    create_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modify_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modify_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_divisi", x => x.div_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_group",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    group_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    group_status = table.Column<int>(type: "int", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    create_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modify_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modify_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_group", x => x.group_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_item",
                columns: table => new
                {
                    item_num = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kat_id = table.Column<int>(type: "int", nullable: false),
                    item_status = table.Column<int>(type: "int", nullable: true),
                    item_desc = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    series_id = table.Column<int>(type: "int", nullable: true),
                    item_na = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    item_asam = table.Column<int>(type: "int", nullable: true),
                    item_ds = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    item_lot = table.Column<int>(type: "int", nullable: true),
                    item_keterangan = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    item_tgl = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    creadate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    creaby = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modidate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modiby = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_item", x => new { x.item_num, x.kat_id });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_job",
                columns: table => new
                {
                    jopr_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    jopr_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    jopr_status = table.Column<int>(type: "int", nullable: true),
                    sec_id = table.Column<int>(type: "int", nullable: true),
                    jopr_alias = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    create_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modify_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modify_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userKasie_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_job", x => x.jopr_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_kategori",
                columns: table => new
                {
                    kat_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    kat_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kat_status = table.Column<int>(type: "int", nullable: true),
                    item_pch = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kat_plant = table.Column<int>(type: "int", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    create_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modify_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modify_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_kategori", x => x.kat_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_line",
                columns: table => new
                {
                    line_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    line_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    line_status = table.Column<int>(type: "int", nullable: true),
                    sec_id = table.Column<int>(type: "int", nullable: true),
                    crea_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    crea_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modi_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modi_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    work_center = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    line_ket = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    line_plant = table.Column<int>(type: "int", nullable: true),
                    userKasie_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_line", x => x.line_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_lineGroup",
                columns: table => new
                {
                    lgp_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    line_id = table.Column<int>(type: "int", nullable: true),
                    group_id = table.Column<int>(type: "int", nullable: true),
                    lgp_status = table.Column<int>(type: "int", nullable: true),
                    lgp_leader = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lgp_kasubsie = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lgp_plant = table.Column<int>(type: "int", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    create_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modify_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modify_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userKasie_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_lineGroup", x => x.lgp_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_mold",
                columns: table => new
                {
                    mold_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mold_description = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    item_num = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mold_sts = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modif_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modif_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    mold_status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_section = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_mold", x => x.mold_code);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_moldtype",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mt_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mt_status = table.Column<int>(type: "int", nullable: true),
                    mt_createBy = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mt_createDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    mt_modifBy = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mt_modifDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    mt_plant = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_moldtype", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_operator",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    jopr_id = table.Column<int>(type: "int", nullable: false),
                    lgp_id = table.Column<int>(type: "int", nullable: true),
                    klasifikasi = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    group_id = table.Column<int>(type: "int", nullable: true),
                    userKasie_id = table.Column<int>(type: "int", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_operator", x => new { x.user_id, x.jopr_id });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_plant",
                columns: table => new
                {
                    plant_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    plant_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    plant_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    plant_status = table.Column<int>(type: "int", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    create_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modify_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modify_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_plant", x => x.plant_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_section",
                columns: table => new
                {
                    sec_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sec_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dep_id = table.Column<int>(type: "int", nullable: true),
                    sec_status = table.Column<int>(type: "int", nullable: true),
                    sec_head = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sec_kode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sec_plant = table.Column<int>(type: "int", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    create_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modify_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modify_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_section", x => x.sec_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_series",
                columns: table => new
                {
                    series_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    series_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modif_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modif_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    series_status = table.Column<int>(type: "int", nullable: true),
                    series_plant = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_series", x => x.series_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_shift",
                columns: table => new
                {
                    shift_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shift_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    shift_start = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    shift_end = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    shift_status = table.Column<int>(type: "int", nullable: true),
                    shift_plan = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    shift_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_shift", x => x.shift_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_user",
                columns: table => new
                {
                    user_npk = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_nama = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_namafull = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_pass = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_email = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_role = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_lastLogin = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    user_createBy = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_modifBy = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_createDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    user_modifDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    user_status = table.Column<int>(type: "int", nullable: true),
                    emp_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dept_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_plant = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_user", x => x.user_npk);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tlkp_userKasie",
                columns: table => new
                {
                    userKasie_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sec_id = table.Column<int>(type: "int", nullable: false),
                    kasie_emp_id = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tlkp_userKasie", x => x.userKasie_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VIEW_DATAAUTH",
                columns: table => new
                {
                    id_recnum = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    emp_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    first_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Full_Name = table.Column<string>(type: "varchar(302)", maxLength: 302, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    emp_no = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<byte>(type: "tinyint unsigned", nullable: true),
                    position_id = table.Column<int>(type: "int", nullable: true),
                    pos_name_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dept_id = table.Column<int>(type: "int", nullable: true),
                    company_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VIEW_DATAAUTH", x => x.id_recnum);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VIEW_EMPLOYEE",
                columns: table => new
                {
                    emp_id = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    plant = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    npk = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dep_id = table.Column<long>(type: "bigint", nullable: true),
                    dep = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sec_id = table.Column<long>(type: "bigint", nullable: true),
                    sec = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_battery_standards",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    battery_type_id = table.Column<int>(type: "int", nullable: false),
                    param_key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    value = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    min_value = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    max_value = table.Column<decimal>(type: "decimal(18,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_battery_standards", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_battery_standards_cos_battery_types_battery_type_id",
                        column: x => x.battery_type_id,
                        principalTable: "cos_battery_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_check_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    form_id = table.Column<int>(type: "int", nullable: false),
                    item_key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    label = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    visual_standard = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numeric_std_key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fixed_standard = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fixed_min = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    fixed_max = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    frequency = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    keterangan = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    conditional_label = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_check_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_check_items_cos_form_definitions_form_id",
                        column: x => x.form_id,
                        principalTable: "cos_form_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_problem_columns",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    form_id = table.Column<int>(type: "int", nullable: false),
                    column_key = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    label = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    field_type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    width = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_problem_columns", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_problem_columns_cos_form_definitions_form_id",
                        column: x => x.form_id,
                        principalTable: "cos_form_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_signature_slots",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    form_id = table.Column<int>(type: "int", nullable: false),
                    role_key = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    label = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_signature_slots", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_signature_slots_cos_form_definitions_form_id",
                        column: x => x.form_id,
                        principalTable: "cos_form_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_submissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    form_id = table.Column<int>(type: "int", nullable: false),
                    tanggal = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    line_id = table.Column<int>(type: "int", nullable: true),
                    shift_id = table.Column<int>(type: "int", nullable: true),
                    operator_emp_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    leader_emp_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kasubsie_emp_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kasie_emp_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    battery_slots_json = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_submissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_submissions_cos_form_definitions_form_id",
                        column: x => x.form_id,
                        principalTable: "cos_form_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_check_sub_rows",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    check_item_id = table.Column<int>(type: "int", nullable: false),
                    suffix = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    label = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fixed_standard = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fixed_min = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    fixed_max = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_check_sub_rows", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_check_sub_rows_cos_check_items_check_item_id",
                        column: x => x.check_item_id,
                        principalTable: "cos_check_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_check_values",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    submission_id = table.Column<int>(type: "int", nullable: false),
                    setting_key = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    value = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_check_values", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_check_values_cos_submissions_submission_id",
                        column: x => x.submission_id,
                        principalTable: "cos_submissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_problems",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    submission_id = table.Column<int>(type: "int", nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    values_json = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_problems", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_problems_cos_submissions_submission_id",
                        column: x => x.submission_id,
                        principalTable: "cos_submissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_signature_entries",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    submission_id = table.Column<int>(type: "int", nullable: false),
                    role_key = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signature_data = table.Column<string>(type: "mediumtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_signature_entries", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_signature_entries_cos_submissions_submission_id",
                        column: x => x.submission_id,
                        principalTable: "cos_submissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "VIEW_DATAAUTH",
                columns: new[] { "id_recnum", "company_id", "dept_id", "email", "emp_id", "emp_no", "first_name", "Full_Name", "last_name", "pos_name_id", "position_id", "status" },
                values: new object[,]
                {
                    { 1L, null, null, null, "EMP-OPR-001", "1001", null, "Ahmad Rizky", null, null, null, (byte)1 },
                    { 2L, null, null, null, "EMP-OPR-002", "1002", null, "Budi Santoso", null, null, null, (byte)1 },
                    { 3L, null, null, null, "EMP-OPR-003", "1003", null, "Cahya Dewi", null, null, null, (byte)1 },
                    { 4L, null, null, null, "EMP-OPR-004", "1004", null, "Dian Permata", null, null, null, (byte)1 },
                    { 5L, null, null, null, "EMP-OPR-005", "1005", null, "Eko Prasetyo", null, null, null, (byte)1 },
                    { 6L, null, null, null, "EMP-OPR-006", "1006", null, "Faisal Rahman", null, null, null, (byte)1 },
                    { 7L, null, null, null, "EMP-LEADER-001", "2001", null, "Hendra Wijaya", null, null, null, (byte)1 },
                    { 8L, null, null, null, "EMP-LEADER-002", "2002", null, "Irfan Hakim", null, null, null, (byte)1 },
                    { 9L, null, null, null, "EMP-KASUBSIE-001", "3001", null, "Joko Prasetyo", null, null, null, (byte)1 },
                    { 10L, null, null, null, "EMP-KASIE-001", "4001", null, "Kurniawan Adi", null, null, null, (byte)1 }
                });

            migrationBuilder.InsertData(
                table: "cos_battery_types",
                columns: new[] { "id", "kat_id", "name" },
                values: new object[,]
                {
                    { 1, null, "NS40ZL" },
                    { 2, null, "NS60L" },
                    { 3, null, "34B19LS" },
                    { 4, null, "N50Z" },
                    { 5, null, "N70Z" },
                    { 6, null, "34B19LS OE TYT" }
                });

            migrationBuilder.InsertData(
                table: "cos_form_definitions",
                columns: new[] { "id", "code", "created_at", "doc_number", "effective_date", "is_active", "revision", "slot_count", "subtitle", "title", "updated_at" },
                values: new object[] { 1, "COS_VALIDATION", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, null, 3, "Form-A2 1-K.051-5-2", "VALIDASI PROSES COS", null });

            migrationBuilder.InsertData(
                table: "tlkp_group",
                columns: new[] { "group_id", "create_by", "create_date", "group_name", "group_status", "modify_by", "modify_date" },
                values: new object[,]
                {
                    { 1, null, null, "Group A", 1, null, null },
                    { 2, null, null, "Group B", 1, null, null },
                    { 3, null, null, "Group C", 1, null, null }
                });

            migrationBuilder.InsertData(
                table: "tlkp_job",
                columns: new[] { "jopr_id", "create_by", "create_date", "jopr_alias", "jopr_name", "jopr_status", "modify_by", "modify_date", "sec_id", "userKasie_id" },
                values: new object[] { 1, null, null, null, "Operator COS", 1, null, null, 1, null });

            migrationBuilder.InsertData(
                table: "tlkp_line",
                columns: new[] { "line_id", "crea_by", "crea_date", "line_ket", "line_name", "line_plant", "line_status", "modi_by", "modi_date", "sec_id", "userKasie_id", "work_center" },
                values: new object[,]
                {
                    { 2, null, null, null, "Line 2", null, 1, null, null, 1, null, null },
                    { 3, null, null, null, "Line 3", null, 1, null, null, 1, null, null },
                    { 4, null, null, null, "Line 4", null, 1, null, null, 1, null, null },
                    { 5, null, null, null, "Line 5", null, 1, null, null, 1, null, null },
                    { 6, null, null, null, "Line 6", null, 1, null, null, 1, null, null },
                    { 7, null, null, null, "Line 7", null, 1, null, null, 1, null, null },
                    { 8, null, null, null, "Line 8", null, 1, null, null, 1, null, null }
                });

            migrationBuilder.InsertData(
                table: "tlkp_lineGroup",
                columns: new[] { "lgp_id", "create_by", "create_date", "group_id", "lgp_kasubsie", "lgp_leader", "lgp_plant", "lgp_status", "line_id", "modify_by", "modify_date", "userKasie_id" },
                values: new object[,]
                {
                    { 1, null, null, 1, "EMP-KASUBSIE-001", "EMP-LEADER-001", null, 1, 2, null, null, 1 },
                    { 2, null, null, 1, "EMP-KASUBSIE-001", "EMP-LEADER-001", null, 1, 3, null, null, 1 },
                    { 3, null, null, 2, "EMP-KASUBSIE-001", "EMP-LEADER-002", null, 1, 4, null, null, 1 },
                    { 4, null, null, 2, "EMP-KASUBSIE-001", "EMP-LEADER-002", null, 1, 5, null, null, 1 }
                });

            migrationBuilder.InsertData(
                table: "tlkp_mold",
                columns: new[] { "mold_code", "created_by", "created_date", "id_section", "item_num", "modif_by", "modif_date", "mold_description", "mold_status", "mold_sts" },
                values: new object[,]
                {
                    { "COS-A01", null, null, 1, null, null, null, "Mold COS A01", "1", null },
                    { "COS-A02", null, null, 1, null, null, null, "Mold COS A02", "1", null },
                    { "COS-B01", null, null, 1, null, null, null, "Mold COS B01", "1", null },
                    { "COS-B02", null, null, 1, null, null, null, "Mold COS B02", "1", null },
                    { "COS-C01", null, null, 1, null, null, null, "Mold COS C01", "1", null },
                    { "COS-C02", null, null, 1, null, null, null, "Mold COS C02", "1", null }
                });

            migrationBuilder.InsertData(
                table: "tlkp_operator",
                columns: new[] { "jopr_id", "user_id", "group_id", "klasifikasi", "lgp_id", "update_date", "userKasie_id" },
                values: new object[,]
                {
                    { 1, "EMP-OPR-001", 1, null, 1, null, 1 },
                    { 1, "EMP-OPR-002", 1, null, 1, null, 1 },
                    { 1, "EMP-OPR-003", 1, null, 2, null, 1 },
                    { 1, "EMP-OPR-004", 2, null, 3, null, 1 },
                    { 1, "EMP-OPR-005", 2, null, 3, null, 1 },
                    { 1, "EMP-OPR-006", 2, null, 4, null, 1 }
                });

            migrationBuilder.InsertData(
                table: "tlkp_section",
                columns: new[] { "sec_id", "create_by", "create_date", "dep_id", "modify_by", "modify_date", "sec_head", "sec_kode", "sec_name", "sec_plant", "sec_status" },
                values: new object[] { 1, null, null, 1, null, null, null, "COS", "COS Section", null, 1 });

            migrationBuilder.InsertData(
                table: "tlkp_shift",
                columns: new[] { "shift_id", "shift_code", "shift_end", "shift_name", "shift_plan", "shift_start", "shift_status" },
                values: new object[,]
                {
                    { 1, "S1", new TimeSpan(0, 15, 0, 0, 0), "Shift 1", null, new TimeSpan(0, 7, 0, 0, 0), 1 },
                    { 2, "S2", new TimeSpan(0, 23, 0, 0, 0), "Shift 2", null, new TimeSpan(0, 15, 0, 0, 0), 1 },
                    { 3, "S3", new TimeSpan(0, 7, 0, 0, 0), "Shift 3", null, new TimeSpan(0, 23, 0, 0, 0), 1 }
                });

            migrationBuilder.InsertData(
                table: "tlkp_userKasie",
                columns: new[] { "userKasie_id", "create_date", "kasie_emp_id", "sec_id" },
                values: new object[] { 1, null, "EMP-KASIE-001", 1 });

            migrationBuilder.InsertData(
                table: "cos_battery_standards",
                columns: new[] { "id", "battery_type_id", "max_value", "min_value", "param_key", "value" },
                values: new object[,]
                {
                    { 1, 1, null, null, "pourWait", "-" },
                    { 2, 2, null, null, "pourWait", "-" },
                    { 3, 3, 2.0m, 1.0m, "pourWait", "1.0 - 2.0" },
                    { 4, 4, null, null, "pourWait", "-" },
                    { 5, 5, null, null, "pourWait", "-" },
                    { 6, 6, 2.0m, 1.0m, "pourWait", "1.0 - 2.0" },
                    { 7, 1, null, null, "pourTime", "" },
                    { 8, 2, 4.0m, 2.5m, "pourTime", "2.5 - 4.0" },
                    { 9, 3, 3.0m, 1.5m, "pourTime", "1.5 - 3.0" },
                    { 10, 4, 5.0m, 3.0m, "pourTime", "3.0 - 5.0" },
                    { 11, 5, 5.5m, 3.5m, "pourTime", "3.5 - 5.5" },
                    { 12, 6, 3.0m, 1.5m, "pourTime", "1.5 - 3.0" },
                    { 13, 1, null, null, "dipTime2", "" },
                    { 14, 2, 3.5m, 2.0m, "dipTime2", "2.0 - 3.5" },
                    { 15, 3, 2.5m, 1.0m, "dipTime2", "1.0 - 2.5" },
                    { 16, 4, 4.0m, 2.5m, "dipTime2", "2.5 - 4.0" },
                    { 17, 5, 4.5m, 3.0m, "dipTime2", "3.0 - 4.5" },
                    { 18, 6, 2.5m, 1.0m, "dipTime2", "1.0 - 2.5" },
                    { 19, 1, null, null, "dumpTime", "" },
                    { 20, 2, 3.0m, 1.5m, "dumpTime", "1.5 - 3.0" },
                    { 21, 3, 2.0m, 0.8m, "dumpTime", "0.8 - 2.0" },
                    { 22, 4, 3.5m, 2.0m, "dumpTime", "2.0 - 3.5" },
                    { 23, 5, 4.0m, 2.5m, "dumpTime", "2.5 - 4.0" },
                    { 24, 6, 2.0m, 0.8m, "dumpTime", "0.8 - 2.0" },
                    { 25, 1, null, null, "lugDryTime", "" },
                    { 26, 2, 5.5m, 3.5m, "lugDryTime", "3.5 - 5.5" },
                    { 27, 3, 4.5m, 2.5m, "lugDryTime", "2.5 - 4.5" },
                    { 28, 4, 6.0m, 4.0m, "lugDryTime", "4.0 - 6.0" },
                    { 29, 5, 6.5m, 4.5m, "lugDryTime", "4.5 - 6.5" },
                    { 30, 6, 4.5m, 2.5m, "lugDryTime", "2.5 - 4.5" },
                    { 31, 1, null, null, "largeVibratorTime", "" },
                    { 32, 2, 3.5m, 1.5m, "largeVibratorTime", "1.5 - 3.5" },
                    { 33, 3, 2.5m, 0.8m, "largeVibratorTime", "0.8 - 2.5" },
                    { 34, 4, 4.0m, 2.0m, "largeVibratorTime", "2.0 - 4.0" },
                    { 35, 5, 4.5m, 2.5m, "largeVibratorTime", "2.5 - 4.5" },
                    { 36, 6, 2.5m, 0.8m, "largeVibratorTime", "0.8 - 2.5" },
                    { 37, 1, null, null, "smallVibratorTime", "" },
                    { 38, 2, 3.5m, 1.5m, "smallVibratorTime", "1.5 - 3.5" },
                    { 39, 3, 2.5m, 0.8m, "smallVibratorTime", "0.8 - 2.5" },
                    { 40, 4, 4.0m, 2.0m, "smallVibratorTime", "2.0 - 4.0" },
                    { 41, 5, 4.5m, 2.5m, "smallVibratorTime", "2.5 - 4.5" },
                    { 42, 6, 2.5m, 0.8m, "smallVibratorTime", "0.8 - 2.5" },
                    { 43, 1, null, null, "coolingTime", "" },
                    { 44, 2, 35m, 25m, "coolingTime", "25 - 35" },
                    { 45, 3, 28m, 18m, "coolingTime", "18 - 28" },
                    { 46, 4, 38m, 28m, "coolingTime", "28 - 38" },
                    { 47, 5, 42m, 30m, "coolingTime", "30 - 42" },
                    { 48, 6, 28m, 18m, "coolingTime", "18 - 28" },
                    { 49, 1, null, null, "leadPumpSpeed", "" },
                    { 50, 2, 65m, 45m, "leadPumpSpeed", "45 - 65" },
                    { 51, 3, 55m, 35m, "leadPumpSpeed", "35 - 55" },
                    { 52, 4, 70m, 50m, "leadPumpSpeed", "50 - 70" },
                    { 53, 5, 75m, 55m, "leadPumpSpeed", "55 - 75" },
                    { 54, 6, 55m, 35m, "leadPumpSpeed", "35 - 55" },
                    { 55, 1, 400m, 300m, "tempAirDryer", "300 - 400" },
                    { 56, 2, 410m, 310m, "tempAirDryer", "310 - 410" },
                    { 57, 3, 390m, 290m, "tempAirDryer", "290 - 390" },
                    { 58, 4, 420m, 320m, "tempAirDryer", "320 - 420" },
                    { 59, 5, 430m, 330m, "tempAirDryer", "330 - 430" },
                    { 60, 6, 390m, 290m, "tempAirDryer", "290 - 390" },
                    { 61, 1, 490m, 470m, "tempPot", "470 - 490" },
                    { 62, 2, 495m, 475m, "tempPot", "475 - 495" },
                    { 63, 3, 485m, 465m, "tempPot", "465 - 485" },
                    { 64, 4, 500m, 480m, "tempPot", "480 - 500" },
                    { 65, 5, 505m, 485m, "tempPot", "485 - 505" },
                    { 66, 6, 485m, 465m, "tempPot", "465 - 485" },
                    { 67, 1, 430m, 410m, "tempPipe", "410 - 430" },
                    { 68, 2, 435m, 415m, "tempPipe", "415 - 435" },
                    { 69, 3, 425m, 405m, "tempPipe", "405 - 425" },
                    { 70, 4, 440m, 420m, "tempPipe", "420 - 440" },
                    { 71, 5, 445m, 425m, "tempPipe", "425 - 445" },
                    { 72, 6, 425m, 405m, "tempPipe", "405 - 425" },
                    { 73, 1, 410m, 390m, "tempCrossBlock", "390 - 410" },
                    { 74, 2, 415m, 395m, "tempCrossBlock", "395 - 415" },
                    { 75, 3, 405m, 385m, "tempCrossBlock", "385 - 405" },
                    { 76, 4, 420m, 400m, "tempCrossBlock", "400 - 420" },
                    { 77, 5, 425m, 405m, "tempCrossBlock", "405 - 425" },
                    { 78, 6, 405m, 385m, "tempCrossBlock", "385 - 405" },
                    { 79, 1, 390m, 370m, "tempElbow", "370 - 390" },
                    { 80, 2, 395m, 375m, "tempElbow", "375 - 395" },
                    { 81, 3, 385m, 365m, "tempElbow", "365 - 385" },
                    { 82, 4, 400m, 380m, "tempElbow", "380 - 400" },
                    { 83, 5, 405m, 385m, "tempElbow", "385 - 405" },
                    { 84, 6, 385m, 365m, "tempElbow", "365 - 385" },
                    { 85, 1, 190m, 160m, "tempMold", "160 - 190" },
                    { 86, 2, 195m, 165m, "tempMold", "165 - 195" },
                    { 87, 3, 185m, 155m, "tempMold", "155 - 185" },
                    { 88, 4, 200m, 170m, "tempMold", "170 - 200" },
                    { 89, 5, 205m, 175m, "tempMold", "175 - 205" },
                    { 90, 6, 185m, 155m, "tempMold", "155 - 185" },
                    { 91, 1, 10m, 6m, "coolingFlowRate", "6 - 10" },
                    { 92, 2, 11m, 7m, "coolingFlowRate", "7 - 11" },
                    { 93, 3, 9m, 5m, "coolingFlowRate", "5 - 9" },
                    { 94, 4, 12m, 8m, "coolingFlowRate", "8 - 12" },
                    { 95, 5, 13m, 9m, "coolingFlowRate", "9 - 13" },
                    { 96, 6, 9m, 5m, "coolingFlowRate", "5 - 9" }
                });

            migrationBuilder.InsertData(
                table: "cos_check_items",
                columns: new[] { "id", "conditional_label", "fixed_max", "fixed_min", "fixed_standard", "form_id", "frequency", "item_key", "keterangan", "label", "numeric_std_key", "sort_order", "type", "visual_standard" },
                values: new object[,]
                {
                    { 1, null, null, null, null, 1, "1 batt / shift / ganti type", "kekuatanCastingStrap", null, "Kekuatan Casting Strap", null, 1, "visual", "Ditarik tidak lepas" },
                    { 2, null, null, null, null, 1, "1 batt / shift / ganti type", "meniscus", null, "Meniscus", null, 2, "visual", "Positif" },
                    { 3, null, null, null, null, 1, "1 Batt / shift / ganti type", "hasilCastingStrap", null, "Hasil Casting Strap", null, 3, "visual", "Tidak ada flash" },
                    { 4, null, null, null, null, 1, null, "levelFlux", null, "Level Flux", null, 4, "visual", "Terisi Flux" },
                    { 5, "Khusus Line 8", null, null, null, 1, "2 x / Shift / ganti type", "pourWait", null, "Pour Wait (Khusus Line 8)", "pourWait", 5, "numeric", null },
                    { 6, null, null, null, null, 1, null, "pourTime", null, "Pour Time", "pourTime", 6, "numeric", null },
                    { 7, null, null, null, null, 1, null, "dipTime2", null, "Dip Time 2", "dipTime2", 7, "numeric", null },
                    { 8, null, null, null, null, 1, null, "dumpTime", null, "Dump Time (Drain back time)", "dumpTime", 8, "numeric", null },
                    { 9, null, null, null, null, 1, "2 x / Shift / ganti type", "lugDryTime", "untuk 34B19LS OE TYT", "Lug Dry Time", "lugDryTime", 9, "numeric", null },
                    { 10, null, null, null, null, 1, null, "largeVibratorTime", null, "Large Vibrator Time", "largeVibratorTime", 10, "numeric", null },
                    { 11, null, null, null, null, 1, null, "smallVibratorTime", null, "Small Vibrator Time", "smallVibratorTime", 11, "numeric", null },
                    { 12, null, null, null, null, 1, null, "coolingTime", null, "Cooling Time", "coolingTime", 12, "numeric", null },
                    { 13, null, null, null, null, 1, null, "leadPumpSpeed", null, "Lead Pump Speed", "leadPumpSpeed", 13, "numeric", null },
                    { 14, null, null, null, null, 1, "1 x / shift", "checkAlignment", null, "Check Alignment", null, 14, "visual", "Bergerak" },
                    { 15, null, null, null, null, 1, "1 x / shift", "checkDatumTable", "Tidak ada ceceran pasta", "Check Datum Table Alignment", null, 15, "visual", "Bersih" },
                    { 16, null, null, null, null, 1, "1 x / shift", "cleaningNozzle", "Spray dengan udara", "Cleaning of Nozzle Lug Dry", null, 16, "visual", "Bersih" },
                    { 17, null, null, 275m, "> 275° C", 1, "2 x / shift", "tempAirNozzleLugDry", "Cek dgn Thermocouple", "Temp Air Nozzle Lug Dry", null, 17, "numeric", null },
                    { 18, null, null, null, null, 1, "2 x / shift", "tempAirDryer", null, "Temp Air Dryer (hot air)", "tempAirDryer", 18, "numeric", null },
                    { 19, "Khusus Line 7", null, 300m, "> 300° C", 1, "2 x / shift", "blowerPipeTemp", null, "Blower Pipe Temp (Khusus Line 7)", null, 19, "numeric", null },
                    { 20, "Khusus Line 7", null, 200m, "> 200° C", 1, "2 x / shift", "blowerNozzle1Temp", null, "Blower Nozzle 1 Temp (Khusus Line 7)", null, 20, "numeric", null },
                    { 21, "Khusus Line 7", null, 200m, "> 200° C", 1, "2 x / shift", "blowerNozzle2Temp", null, "Blower Nozzle 2 Temp (Khusus Line 7)", null, 21, "numeric", null },
                    { 22, null, null, null, null, 1, "2 x / shift", "tempPot", null, "Temperatur Pot", "tempPot", 22, "numeric", null },
                    { 23, null, null, null, null, 1, "2 x / shift", "tempPipe", null, "Temperatur Pipe", "tempPipe", 23, "numeric", null },
                    { 24, null, null, null, null, 1, "2 x / shift", "tempCrossBlock", null, "Temp. Cross Block", "tempCrossBlock", 24, "numeric", null },
                    { 25, null, null, null, null, 1, "2 x / shift", "tempElbow", null, "Temp. Elbow (Lead Lump)", "tempElbow", 25, "numeric", null },
                    { 26, null, null, null, null, 1, "2 x / shift", "tempMold", null, "Temperatur Mold", "tempMold", 26, "numeric", null },
                    { 27, null, null, null, null, 1, "2 x / shift", "coolingFlowRate", null, "Cooling Water Flow Rate", "coolingFlowRate", 27, "numeric", null },
                    { 28, null, 30m, 26m, "28 ± 2 °C", 1, "2 x / shift", "coolingWaterTemp", null, "Cooling Water Temperature", null, 28, "numeric", null },
                    { 29, null, null, null, null, 1, "2 x / Shift", "sprueBrush", null, "Sprue Brush", null, 29, "visual", "Berfungsi dengan baik" },
                    { 30, null, null, null, null, 1, "3 x / Shift", "cleaningCavityMold", null, "Cleaning Cavity Mold COS", null, 30, "visual", "Tidak tersumbat dross" },
                    { 31, null, null, null, null, 1, "1 batt / shift / ganti type", "fluxTime", null, "Flux Time", null, 31, "numeric", null },
                    { 32, null, null, null, null, 1, "1 batt / shift / ganti type", "overflowHydrazine", null, "Overflow Hydrazine", null, 32, "numeric", null }
                });

            migrationBuilder.InsertData(
                table: "cos_problem_columns",
                columns: new[] { "id", "column_key", "field_type", "form_id", "label", "sort_order", "width" },
                values: new object[,]
                {
                    { 1, "item", "text", 1, "ITEM", 1, "120px" },
                    { 2, "masalah", "text", 1, "MASALAH", 2, "200px" },
                    { 3, "tindakan", "text", 1, "TINDAKAN", 3, "200px" },
                    { 4, "waktu", "text", 1, "WAKTU", 4, "80px" },
                    { 5, "menit", "number", 1, "MENIT", 5, "60px" },
                    { 6, "pic", "text", 1, "PIC", 6, "100px" }
                });

            migrationBuilder.InsertData(
                table: "cos_signature_slots",
                columns: new[] { "id", "form_id", "label", "role_key", "sort_order" },
                values: new object[,]
                {
                    { 1, 1, "Dibuat", "operator", 1 },
                    { 2, 1, "Diperiksa", "leader", 2 },
                    { 3, 1, "Diketahui", "kasubsie", 3 },
                    { 4, 1, "Disetujui", "kasie", 4 }
                });

            migrationBuilder.InsertData(
                table: "cos_check_sub_rows",
                columns: new[] { "id", "check_item_id", "fixed_max", "fixed_min", "fixed_standard", "label", "sort_order", "suffix" },
                values: new object[,]
                {
                    { 1, 1, null, null, null, "+", 1, "plus" },
                    { 2, 1, null, null, null, "−", 2, "minus" },
                    { 3, 2, null, null, null, "+", 1, "plus" },
                    { 4, 2, null, null, null, "−", 2, "minus" },
                    { 5, 23, null, null, null, "L", 1, "L" },
                    { 6, 23, null, null, null, "R", 2, "R" },
                    { 7, 26, null, null, null, "Mold 1", 1, "mold1" },
                    { 8, 26, null, null, null, "Mold 2", 2, "mold2" },
                    { 9, 26, null, null, null, "Post 1", 3, "post1" },
                    { 10, 26, null, null, null, "Post 2", 4, "post2" },
                    { 11, 27, null, null, null, "Mold 1", 1, "mold1" },
                    { 12, 27, null, null, null, "Mold 2", 2, "mold2" },
                    { 13, 31, 3m, 1m, "1 - 3 detik", "Line 6", 1, "line6" },
                    { 14, 31, 1m, 0.1m, "0.1 - 1 detik", "Line 2,3,4,5,7&8", 2, "lineOther" },
                    { 15, 32, 10m, 10m, "10 detik", "Line 2", 1, "line2" },
                    { 16, 32, 5m, 5m, "5 detik", "Line 7", 2, "line7" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_cos_battery_standards_battery_type_id_param_key",
                table: "cos_battery_standards",
                columns: new[] { "battery_type_id", "param_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cos_check_items_form_id_item_key",
                table: "cos_check_items",
                columns: new[] { "form_id", "item_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cos_check_sub_rows_check_item_id",
                table: "cos_check_sub_rows",
                column: "check_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_cos_check_values_submission_id_setting_key",
                table: "cos_check_values",
                columns: new[] { "submission_id", "setting_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cos_form_definitions_code",
                table: "cos_form_definitions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cos_problem_columns_form_id",
                table: "cos_problem_columns",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "IX_cos_problems_submission_id",
                table: "cos_problems",
                column: "submission_id");

            migrationBuilder.CreateIndex(
                name: "IX_cos_signature_entries_submission_id_role_key",
                table: "cos_signature_entries",
                columns: new[] { "submission_id", "role_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cos_signature_slots_form_id",
                table: "cos_signature_slots",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "IX_cos_submissions_form_id",
                table: "cos_submissions",
                column: "form_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cos_battery_standards");

            migrationBuilder.DropTable(
                name: "cos_check_sub_rows");

            migrationBuilder.DropTable(
                name: "cos_check_values");

            migrationBuilder.DropTable(
                name: "cos_problem_columns");

            migrationBuilder.DropTable(
                name: "cos_problems");

            migrationBuilder.DropTable(
                name: "cos_signature_entries");

            migrationBuilder.DropTable(
                name: "cos_signature_slots");

            migrationBuilder.DropTable(
                name: "tlkp_departemen");

            migrationBuilder.DropTable(
                name: "tlkp_divisi");

            migrationBuilder.DropTable(
                name: "tlkp_group");

            migrationBuilder.DropTable(
                name: "tlkp_item");

            migrationBuilder.DropTable(
                name: "tlkp_job");

            migrationBuilder.DropTable(
                name: "tlkp_kategori");

            migrationBuilder.DropTable(
                name: "tlkp_line");

            migrationBuilder.DropTable(
                name: "tlkp_lineGroup");

            migrationBuilder.DropTable(
                name: "tlkp_mold");

            migrationBuilder.DropTable(
                name: "tlkp_moldtype");

            migrationBuilder.DropTable(
                name: "tlkp_operator");

            migrationBuilder.DropTable(
                name: "tlkp_plant");

            migrationBuilder.DropTable(
                name: "tlkp_section");

            migrationBuilder.DropTable(
                name: "tlkp_series");

            migrationBuilder.DropTable(
                name: "tlkp_shift");

            migrationBuilder.DropTable(
                name: "tlkp_user");

            migrationBuilder.DropTable(
                name: "tlkp_userKasie");

            migrationBuilder.DropTable(
                name: "VIEW_DATAAUTH");

            migrationBuilder.DropTable(
                name: "VIEW_EMPLOYEE");

            migrationBuilder.DropTable(
                name: "cos_battery_types");

            migrationBuilder.DropTable(
                name: "cos_check_items");

            migrationBuilder.DropTable(
                name: "cos_submissions");

            migrationBuilder.DropTable(
                name: "cos_form_definitions");
        }
    }
}
