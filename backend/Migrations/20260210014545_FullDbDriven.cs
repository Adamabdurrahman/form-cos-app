using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FullDbDriven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "battery_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_battery_type", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "form_definition",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    subtitle = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slot_count = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_form_definition", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "kasie",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kasie", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "battery_mold",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    battery_type_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_battery_mold", x => x.id);
                    table.ForeignKey(
                        name: "FK_battery_mold_battery_type_battery_type_id",
                        column: x => x.battery_type_id,
                        principalTable: "battery_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "battery_standard",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    param_key = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    value = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    min_value = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    max_value = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    battery_type_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_battery_standard", x => x.id);
                    table.ForeignKey(
                        name: "FK_battery_standard_battery_type_battery_type_id",
                        column: x => x.battery_type_id,
                        principalTable: "battery_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "check_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    form_id = table.Column<int>(type: "int", nullable: false),
                    item_key = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    label = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    visual_standard = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numeric_std_key = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fixed_standard = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fixed_min = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    fixed_max = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    frequency = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    keterangan = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    conditional_label = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_check_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_check_item_form_definition_form_id",
                        column: x => x.form_id,
                        principalTable: "form_definition",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "form_problem_column",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    column_key = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    label = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    field_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    width = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    form_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_form_problem_column", x => x.id);
                    table.ForeignKey(
                        name: "FK_form_problem_column_form_definition_form_id",
                        column: x => x.form_id,
                        principalTable: "form_definition",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "form_signature_slot",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    role_key = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    label = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    form_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_form_signature_slot", x => x.id);
                    table.ForeignKey(
                        name: "FK_form_signature_slot_form_definition_form_id",
                        column: x => x.form_id,
                        principalTable: "form_definition",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "kasubsie",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kasie_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kasubsie", x => x.id);
                    table.ForeignKey(
                        name: "FK_kasubsie_kasie_kasie_id",
                        column: x => x.kasie_id,
                        principalTable: "kasie",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "check_sub_row",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    suffix = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    label = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fixed_standard = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fixed_min = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    fixed_max = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    check_item_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_check_sub_row", x => x.id);
                    table.ForeignKey(
                        name: "FK_check_sub_row_check_item_check_item_id",
                        column: x => x.check_item_id,
                        principalTable: "check_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "leader",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kasubsie_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leader", x => x.id);
                    table.ForeignKey(
                        name: "FK_leader_kasubsie_kasubsie_id",
                        column: x => x.kasubsie_id,
                        principalTable: "kasubsie",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "operator",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    leader_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operator", x => x.id);
                    table.ForeignKey(
                        name: "FK_operator_leader_leader_id",
                        column: x => x.leader_id,
                        principalTable: "leader",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "form_submission",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    form_id = table.Column<int>(type: "int", nullable: false),
                    tanggal = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    line = table.Column<int>(type: "int", nullable: false),
                    shift = table.Column<int>(type: "int", nullable: false),
                    operator_id = table.Column<int>(type: "int", nullable: false),
                    leader_id = table.Column<int>(type: "int", nullable: true),
                    kasubsie_id = table.Column<int>(type: "int", nullable: true),
                    kasie_id = table.Column<int>(type: "int", nullable: true),
                    battery_slots_json = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_form_submission", x => x.id);
                    table.ForeignKey(
                        name: "FK_form_submission_form_definition_form_id",
                        column: x => x.form_id,
                        principalTable: "form_definition",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_form_submission_kasie_kasie_id",
                        column: x => x.kasie_id,
                        principalTable: "kasie",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_form_submission_kasubsie_kasubsie_id",
                        column: x => x.kasubsie_id,
                        principalTable: "kasubsie",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_form_submission_leader_leader_id",
                        column: x => x.leader_id,
                        principalTable: "leader",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_form_submission_operator_operator_id",
                        column: x => x.operator_id,
                        principalTable: "operator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "submission_check_value",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    setting_key = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    value = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    submission_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submission_check_value", x => x.id);
                    table.ForeignKey(
                        name: "FK_submission_check_value_form_submission_submission_id",
                        column: x => x.submission_id,
                        principalTable: "form_submission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "submission_problem",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    values_json = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    submission_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submission_problem", x => x.id);
                    table.ForeignKey(
                        name: "FK_submission_problem_form_submission_submission_id",
                        column: x => x.submission_id,
                        principalTable: "form_submission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "submission_signature",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    role_key = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signature_data = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    submission_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submission_signature", x => x.id);
                    table.ForeignKey(
                        name: "FK_submission_signature_form_submission_submission_id",
                        column: x => x.submission_id,
                        principalTable: "form_submission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "battery_type",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "NS40ZL" },
                    { 2, "NS60L" },
                    { 3, "34B19LS" },
                    { 4, "N50Z" },
                    { 5, "N70Z" },
                    { 6, "34B19LS OE TYT" }
                });

            migrationBuilder.InsertData(
                table: "form_definition",
                columns: new[] { "id", "code", "created_at", "is_active", "slot_count", "subtitle", "title", "updated_at" },
                values: new object[] { 1, "COS_VALIDATION", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, "Form-A2 1-K.051-5-2", "VALIDASI PROSES COS", null });

            migrationBuilder.InsertData(
                table: "kasie",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Kurniawan Adi" },
                    { 2, "Sutrisno Hadi" }
                });

            migrationBuilder.InsertData(
                table: "battery_mold",
                columns: new[] { "id", "battery_type_id", "name" },
                values: new object[,]
                {
                    { 1, 1, "COS-A01" },
                    { 2, 1, "COS-A02" },
                    { 3, 1, "COS-A03" },
                    { 4, 2, "COS-B01" },
                    { 5, 2, "COS-B02" },
                    { 6, 3, "COS-C01" },
                    { 7, 3, "COS-C02" },
                    { 8, 3, "COS-C03" },
                    { 9, 4, "COS-D01" },
                    { 10, 4, "COS-D02" },
                    { 11, 5, "COS-E01" },
                    { 12, 5, "COS-E02" },
                    { 13, 5, "COS-E03" },
                    { 14, 6, "COS-F01" },
                    { 15, 6, "COS-F02" }
                });

            migrationBuilder.InsertData(
                table: "battery_standard",
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
                table: "check_item",
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
                table: "form_problem_column",
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
                table: "form_signature_slot",
                columns: new[] { "id", "form_id", "label", "role_key", "sort_order" },
                values: new object[,]
                {
                    { 1, 1, "Dibuat", "operator", 1 },
                    { 2, 1, "Diperiksa", "leader", 2 },
                    { 3, 1, "Diketahui", "kasubsie", 3 },
                    { 4, 1, "Disetujui", "kasie", 4 }
                });

            migrationBuilder.InsertData(
                table: "kasubsie",
                columns: new[] { "id", "kasie_id", "name" },
                values: new object[,]
                {
                    { 1, 1, "Joko Prasetyo" },
                    { 2, 1, "Mulyono Slamet" },
                    { 3, 2, "Teguh Wibowo" }
                });

            migrationBuilder.InsertData(
                table: "check_sub_row",
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

            migrationBuilder.InsertData(
                table: "leader",
                columns: new[] { "id", "kasubsie_id", "name" },
                values: new object[,]
                {
                    { 1, 1, "Hendra Wijaya" },
                    { 2, 1, "Irfan Hakim" },
                    { 3, 2, "Fajar Nugroho" },
                    { 4, 3, "Dedi Kurniawan" }
                });

            migrationBuilder.InsertData(
                table: "operator",
                columns: new[] { "id", "leader_id", "name" },
                values: new object[,]
                {
                    { 1, 1, "Ahmad Rizky" },
                    { 2, 1, "Budi Santoso" },
                    { 3, 2, "Cahya Dewi" },
                    { 4, 2, "Dian Permata" },
                    { 5, 3, "Eko Prasetyo" },
                    { 6, 3, "Faisal Rahman" },
                    { 7, 4, "Gunawan Putra" },
                    { 8, 4, "Haris Munandar" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_battery_mold_battery_type_id",
                table: "battery_mold",
                column: "battery_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_battery_standard_battery_type_id_param_key",
                table: "battery_standard",
                columns: new[] { "battery_type_id", "param_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_check_item_form_id_item_key",
                table: "check_item",
                columns: new[] { "form_id", "item_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_check_sub_row_check_item_id",
                table: "check_sub_row",
                column: "check_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_form_definition_code",
                table: "form_definition",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_form_problem_column_form_id",
                table: "form_problem_column",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "IX_form_signature_slot_form_id",
                table: "form_signature_slot",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "IX_form_submission_form_id",
                table: "form_submission",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "IX_form_submission_kasie_id",
                table: "form_submission",
                column: "kasie_id");

            migrationBuilder.CreateIndex(
                name: "IX_form_submission_kasubsie_id",
                table: "form_submission",
                column: "kasubsie_id");

            migrationBuilder.CreateIndex(
                name: "IX_form_submission_leader_id",
                table: "form_submission",
                column: "leader_id");

            migrationBuilder.CreateIndex(
                name: "IX_form_submission_operator_id",
                table: "form_submission",
                column: "operator_id");

            migrationBuilder.CreateIndex(
                name: "IX_kasubsie_kasie_id",
                table: "kasubsie",
                column: "kasie_id");

            migrationBuilder.CreateIndex(
                name: "IX_leader_kasubsie_id",
                table: "leader",
                column: "kasubsie_id");

            migrationBuilder.CreateIndex(
                name: "IX_operator_leader_id",
                table: "operator",
                column: "leader_id");

            migrationBuilder.CreateIndex(
                name: "IX_submission_check_value_submission_id_setting_key",
                table: "submission_check_value",
                columns: new[] { "submission_id", "setting_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_submission_problem_submission_id",
                table: "submission_problem",
                column: "submission_id");

            migrationBuilder.CreateIndex(
                name: "IX_submission_signature_submission_id_role_key",
                table: "submission_signature",
                columns: new[] { "submission_id", "role_key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "battery_mold");

            migrationBuilder.DropTable(
                name: "battery_standard");

            migrationBuilder.DropTable(
                name: "check_sub_row");

            migrationBuilder.DropTable(
                name: "form_problem_column");

            migrationBuilder.DropTable(
                name: "form_signature_slot");

            migrationBuilder.DropTable(
                name: "submission_check_value");

            migrationBuilder.DropTable(
                name: "submission_problem");

            migrationBuilder.DropTable(
                name: "submission_signature");

            migrationBuilder.DropTable(
                name: "battery_type");

            migrationBuilder.DropTable(
                name: "check_item");

            migrationBuilder.DropTable(
                name: "form_submission");

            migrationBuilder.DropTable(
                name: "form_definition");

            migrationBuilder.DropTable(
                name: "operator");

            migrationBuilder.DropTable(
                name: "leader");

            migrationBuilder.DropTable(
                name: "kasubsie");

            migrationBuilder.DropTable(
                name: "kasie");
        }
    }
}
