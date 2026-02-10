using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                name: "check_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                name: "cos_validation",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tanggal = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    line = table.Column<int>(type: "int", nullable: false),
                    shift = table.Column<int>(type: "int", nullable: false),
                    operator_id = table.Column<int>(type: "int", nullable: false),
                    leader_id = table.Column<int>(type: "int", nullable: true),
                    kasubsie_id = table.Column<int>(type: "int", nullable: true),
                    kasie_id = table.Column<int>(type: "int", nullable: true),
                    battery_type_1 = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mold_1 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    battery_type_2 = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mold_2 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    battery_type_3 = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mold_3 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_validation", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_validation_kasie_kasie_id",
                        column: x => x.kasie_id,
                        principalTable: "kasie",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_cos_validation_kasubsie_kasubsie_id",
                        column: x => x.kasubsie_id,
                        principalTable: "kasubsie",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_cos_validation_leader_leader_id",
                        column: x => x.leader_id,
                        principalTable: "leader",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_cos_validation_operator_operator_id",
                        column: x => x.operator_id,
                        principalTable: "operator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_check_setting",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    setting_key = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    value = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cos_validation_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_check_setting", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_check_setting_cos_validation_cos_validation_id",
                        column: x => x.cos_validation_id,
                        principalTable: "cos_validation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_problem",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    problem = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    cos_validation_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_problem", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_problem_cos_validation_cos_validation_id",
                        column: x => x.cos_validation_id,
                        principalTable: "cos_validation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cos_signature",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    role = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signature_data = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cos_validation_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_signature", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_signature_cos_validation_cos_validation_id",
                        column: x => x.cos_validation_id,
                        principalTable: "cos_validation",
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
                table: "check_item",
                columns: new[] { "id", "conditional_label", "fixed_standard", "frequency", "item_key", "keterangan", "label", "numeric_std_key", "sort_order", "type", "visual_standard" },
                values: new object[,]
                {
                    { 1, null, null, "1 batt / shift / ganti type", "kekuatanCastingStrap", null, "Kekuatan Casting Strap", null, 1, "visual", "Ditarik tidak lepas" },
                    { 2, null, null, "1 batt / shift / ganti type", "meniscus", null, "Meniscus", null, 2, "visual", "Positif" },
                    { 3, null, null, "1 Batt / shift / ganti type", "hasilCastingStrap", null, "Hasil Casting Strap", null, 3, "visual", "Tidak ada flash" },
                    { 4, null, null, "", "levelFlux", null, "Level Flux", null, 4, "visual", "Terisi Flux" },
                    { 5, "Khusus Line 8", null, "2 x / Shift / ganti type", "pourWait", null, "Pour Wait (Khusus Line 8)", "pourWait", 5, "numeric", null },
                    { 6, null, null, "", "pourTime", null, "Pour Time", "pourTime", 6, "numeric", null },
                    { 7, null, null, "", "dipTime2", null, "Dip Time 2", "dipTime2", 7, "numeric", null },
                    { 8, null, null, "", "dumpTime", null, "Dump Time (Drain back time)", "dumpTime", 8, "numeric", null },
                    { 9, null, null, "2 x / Shift / ganti type", "lugDryTime", "untuk 34B19LS OE TYT", "Lug Dry Time", "lugDryTime", 9, "numeric", null },
                    { 10, null, null, "", "largeVibratorTime", null, "Large Vibrator Time", "largeVibratorTime", 10, "numeric", null },
                    { 11, null, null, "", "smallVibratorTime", null, "Small Vibrator Time", "smallVibratorTime", 11, "numeric", null },
                    { 12, null, null, "", "coolingTime", null, "Cooling Time", "coolingTime", 12, "numeric", null },
                    { 13, null, null, "", "leadPumpSpeed", null, "Lead Pump Speed", "leadPumpSpeed", 13, "numeric", null },
                    { 14, null, null, "1 x / shift", "checkAlignment", null, "Check Alignment", null, 14, "visual", "Bergerak" },
                    { 15, null, null, "1 x / shift", "checkDatumTable", "Tidak ada ceceran pasta", "Check Datum Table Alignment", null, 15, "visual", "Bersih" },
                    { 16, null, null, "1 x / shift", "cleaningNozzle", "Spray dengan udara", "Cleaning of Nozzle Lug Dry", null, 16, "visual", "Bersih" },
                    { 17, null, "> 275° C", "2 x / shift", "tempAirNozzleLugDry", "Cek dgn Thermocouple", "Temp Air Nozzle Lug Dry", null, 17, "numeric", null },
                    { 18, null, null, "2 x / shift", "tempAirDryer", null, "Temp Air Dryer (hot air)", "tempAirDryer", 18, "numeric", null },
                    { 19, "Khusus Line 7", "> 300° C", "2 x / shift", "blowerPipeTemp", null, "Blower Pipe Temp (Khusus Line 7)", null, 19, "numeric", null },
                    { 20, "Khusus Line 7", "> 200° C", "2 x / shift", "blowerNozzle1Temp", null, "Blower Nozzle 1 Temp (Khusus Line 7)", null, 20, "numeric", null },
                    { 21, "Khusus Line 7", "> 200° C", "2 x / shift", "blowerNozzle2Temp", null, "Blower Nozzle 2 Temp (Khusus Line 7)", null, 21, "numeric", null },
                    { 22, null, null, "2 x / shift", "tempPot", null, "Temperatur Pot", "tempPot", 22, "numeric", null },
                    { 23, null, null, "2 x / shift", "tempPipe", null, "Temperatur Pipe", "tempPipe", 23, "numeric", null },
                    { 24, null, null, "2 x / shift", "tempCrossBlock", null, "Temp. Cross Block", "tempCrossBlock", 24, "numeric", null },
                    { 25, null, null, "2 x / shift", "tempElbow", null, "Temp. Elbow (Lead Lump)", "tempElbow", 25, "numeric", null },
                    { 26, null, null, "2 x / shift", "tempMold", null, "Temperatur Mold", "tempMold", 26, "numeric", null },
                    { 27, null, null, "2 x / shift", "coolingFlowRate", null, "Cooling Water Flow Rate", "coolingFlowRate", 27, "numeric", null },
                    { 28, null, "28 ± 2 °C", "2 x / shift", "coolingWaterTemp", null, "Cooling Water Temperature", null, 28, "numeric", null },
                    { 29, null, null, "2 x / Shift", "sprueBrush", null, "Sprue Brush", null, 29, "visual", "Berfungsi dengan baik" },
                    { 30, null, null, "3 x / Shift", "cleaningCavityMold", null, "Cleaning Cavity Mold COS", null, 30, "visual", "Tidak tersumbat dross" },
                    { 31, null, null, "1 batt / shift / ganti type", "fluxTime", null, "Flux Time", null, 31, "numeric", null },
                    { 32, null, null, "1 batt / shift / ganti type", "overflowHydrazine", null, "Overflow Hydrazine", null, 32, "numeric", null }
                });

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
                columns: new[] { "id", "battery_type_id", "param_key", "value" },
                values: new object[,]
                {
                    { 1, 1, "pourWait", "-" },
                    { 2, 2, "pourWait", "-" },
                    { 3, 3, "pourWait", "1.0 - 2.0" },
                    { 4, 4, "pourWait", "-" },
                    { 5, 5, "pourWait", "-" },
                    { 6, 6, "pourWait", "1.0 - 2.0" },
                    { 7, 1, "pourTime", "" },
                    { 8, 2, "pourTime", "2.5 - 4.0" },
                    { 9, 3, "pourTime", "1.5 - 3.0" },
                    { 10, 4, "pourTime", "3.0 - 5.0" },
                    { 11, 5, "pourTime", "3.5 - 5.5" },
                    { 12, 6, "pourTime", "1.5 - 3.0" },
                    { 13, 1, "dipTime2", "" },
                    { 14, 2, "dipTime2", "2.0 - 3.5" },
                    { 15, 3, "dipTime2", "1.0 - 2.5" },
                    { 16, 4, "dipTime2", "2.5 - 4.0" },
                    { 17, 5, "dipTime2", "3.0 - 4.5" },
                    { 18, 6, "dipTime2", "1.0 - 2.5" },
                    { 19, 1, "dumpTime", "" },
                    { 20, 2, "dumpTime", "1.5 - 3.0" },
                    { 21, 3, "dumpTime", "0.8 - 2.0" },
                    { 22, 4, "dumpTime", "2.0 - 3.5" },
                    { 23, 5, "dumpTime", "2.5 - 4.0" },
                    { 24, 6, "dumpTime", "0.8 - 2.0" },
                    { 25, 1, "lugDryTime", "" },
                    { 26, 2, "lugDryTime", "3.5 - 5.5" },
                    { 27, 3, "lugDryTime", "2.5 - 4.5" },
                    { 28, 4, "lugDryTime", "4.0 - 6.0" },
                    { 29, 5, "lugDryTime", "4.5 - 6.5" },
                    { 30, 6, "lugDryTime", "2.5 - 4.5" },
                    { 31, 1, "largeVibratorTime", "" },
                    { 32, 2, "largeVibratorTime", "1.5 - 3.5" },
                    { 33, 3, "largeVibratorTime", "0.8 - 2.5" },
                    { 34, 4, "largeVibratorTime", "2.0 - 4.0" },
                    { 35, 5, "largeVibratorTime", "2.5 - 4.5" },
                    { 36, 6, "largeVibratorTime", "0.8 - 2.5" },
                    { 37, 1, "smallVibratorTime", "" },
                    { 38, 2, "smallVibratorTime", "1.5 - 3.5" },
                    { 39, 3, "smallVibratorTime", "0.8 - 2.5" },
                    { 40, 4, "smallVibratorTime", "2.0 - 4.0" },
                    { 41, 5, "smallVibratorTime", "2.5 - 4.5" },
                    { 42, 6, "smallVibratorTime", "0.8 - 2.5" },
                    { 43, 1, "coolingTime", "" },
                    { 44, 2, "coolingTime", "25 - 35" },
                    { 45, 3, "coolingTime", "18 - 28" },
                    { 46, 4, "coolingTime", "28 - 38" },
                    { 47, 5, "coolingTime", "30 - 42" },
                    { 48, 6, "coolingTime", "18 - 28" },
                    { 49, 1, "leadPumpSpeed", "" },
                    { 50, 2, "leadPumpSpeed", "45 - 65" },
                    { 51, 3, "leadPumpSpeed", "35 - 55" },
                    { 52, 4, "leadPumpSpeed", "50 - 70" },
                    { 53, 5, "leadPumpSpeed", "55 - 75" },
                    { 54, 6, "leadPumpSpeed", "35 - 55" },
                    { 55, 1, "tempAirDryer", "300 - 400" },
                    { 56, 2, "tempAirDryer", "310 - 410" },
                    { 57, 3, "tempAirDryer", "290 - 390" },
                    { 58, 4, "tempAirDryer", "320 - 420" },
                    { 59, 5, "tempAirDryer", "330 - 430" },
                    { 60, 6, "tempAirDryer", "290 - 390" },
                    { 61, 1, "tempPot", "470 - 490" },
                    { 62, 2, "tempPot", "475 - 495" },
                    { 63, 3, "tempPot", "465 - 485" },
                    { 64, 4, "tempPot", "480 - 500" },
                    { 65, 5, "tempPot", "485 - 505" },
                    { 66, 6, "tempPot", "465 - 485" },
                    { 67, 1, "tempPipe", "410 - 430" },
                    { 68, 2, "tempPipe", "415 - 435" },
                    { 69, 3, "tempPipe", "405 - 425" },
                    { 70, 4, "tempPipe", "420 - 440" },
                    { 71, 5, "tempPipe", "425 - 445" },
                    { 72, 6, "tempPipe", "405 - 425" },
                    { 73, 1, "tempCrossBlock", "390 - 410" },
                    { 74, 2, "tempCrossBlock", "395 - 415" },
                    { 75, 3, "tempCrossBlock", "385 - 405" },
                    { 76, 4, "tempCrossBlock", "400 - 420" },
                    { 77, 5, "tempCrossBlock", "405 - 425" },
                    { 78, 6, "tempCrossBlock", "385 - 405" },
                    { 79, 1, "tempElbow", "370 - 390" },
                    { 80, 2, "tempElbow", "375 - 395" },
                    { 81, 3, "tempElbow", "365 - 385" },
                    { 82, 4, "tempElbow", "380 - 400" },
                    { 83, 5, "tempElbow", "385 - 405" },
                    { 84, 6, "tempElbow", "365 - 385" },
                    { 85, 1, "tempMold", "160 - 190" },
                    { 86, 2, "tempMold", "165 - 195" },
                    { 87, 3, "tempMold", "155 - 185" },
                    { 88, 4, "tempMold", "170 - 200" },
                    { 89, 5, "tempMold", "175 - 205" },
                    { 90, 6, "tempMold", "155 - 185" },
                    { 91, 1, "coolingFlowRate", "6 - 10" },
                    { 92, 2, "coolingFlowRate", "7 - 11" },
                    { 93, 3, "coolingFlowRate", "5 - 9" },
                    { 94, 4, "coolingFlowRate", "8 - 12" },
                    { 95, 5, "coolingFlowRate", "9 - 13" },
                    { 96, 6, "coolingFlowRate", "5 - 9" }
                });

            migrationBuilder.InsertData(
                table: "check_sub_row",
                columns: new[] { "id", "check_item_id", "fixed_standard", "label", "sort_order", "suffix" },
                values: new object[,]
                {
                    { 1, 1, null, "+", 1, "plus" },
                    { 2, 1, null, "−", 2, "minus" },
                    { 3, 2, null, "+", 1, "plus" },
                    { 4, 2, null, "−", 2, "minus" },
                    { 5, 23, null, "L", 1, "L" },
                    { 6, 23, null, "R", 2, "R" },
                    { 7, 26, null, "Mold 1", 1, "mold1" },
                    { 8, 26, null, "Mold 2", 2, "mold2" },
                    { 9, 26, null, "Post 1", 3, "post1" },
                    { 10, 26, null, "Post 2", 4, "post2" },
                    { 11, 27, null, "Mold 1", 1, "mold1" },
                    { 12, 27, null, "Mold 2", 2, "mold2" },
                    { 13, 31, "1 - 3 detik", "Line 6", 1, "line6" },
                    { 14, 31, "0.1 - 1 detik", "Line 2,3,4,5,7&8", 2, "lineOther" },
                    { 15, 32, "10 detik", "Line 2", 1, "line2" },
                    { 16, 32, "5 detik", "Line 7", 2, "line7" }
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
                name: "IX_check_item_item_key",
                table: "check_item",
                column: "item_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_check_sub_row_check_item_id",
                table: "check_sub_row",
                column: "check_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_cos_check_setting_cos_validation_id_setting_key",
                table: "cos_check_setting",
                columns: new[] { "cos_validation_id", "setting_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cos_problem_cos_validation_id",
                table: "cos_problem",
                column: "cos_validation_id");

            migrationBuilder.CreateIndex(
                name: "IX_cos_signature_cos_validation_id_role",
                table: "cos_signature",
                columns: new[] { "cos_validation_id", "role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cos_validation_kasie_id",
                table: "cos_validation",
                column: "kasie_id");

            migrationBuilder.CreateIndex(
                name: "IX_cos_validation_kasubsie_id",
                table: "cos_validation",
                column: "kasubsie_id");

            migrationBuilder.CreateIndex(
                name: "IX_cos_validation_leader_id",
                table: "cos_validation",
                column: "leader_id");

            migrationBuilder.CreateIndex(
                name: "IX_cos_validation_operator_id",
                table: "cos_validation",
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
                name: "cos_check_setting");

            migrationBuilder.DropTable(
                name: "cos_problem");

            migrationBuilder.DropTable(
                name: "cos_signature");

            migrationBuilder.DropTable(
                name: "battery_type");

            migrationBuilder.DropTable(
                name: "check_item");

            migrationBuilder.DropTable(
                name: "cos_validation");

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
