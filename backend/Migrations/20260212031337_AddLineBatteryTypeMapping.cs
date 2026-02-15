using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddLineBatteryTypeMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cos_line_battery_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    line_id = table.Column<int>(type: "int", nullable: false),
                    battery_type_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_line_battery_types", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_line_battery_types_cos_battery_types_battery_type_id",
                        column: x => x.battery_type_id,
                        principalTable: "cos_battery_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "cos_line_battery_types",
                columns: new[] { "id", "battery_type_id", "line_id" },
                values: new object[,]
                {
                    { 1, 1, 2 },
                    { 2, 2, 2 },
                    { 3, 3, 2 },
                    { 4, 1, 3 },
                    { 5, 2, 3 },
                    { 6, 3, 3 },
                    { 7, 1, 4 },
                    { 8, 2, 4 },
                    { 9, 3, 4 },
                    { 10, 1, 5 },
                    { 11, 2, 5 },
                    { 12, 3, 5 },
                    { 13, 4, 6 },
                    { 14, 5, 6 },
                    { 15, 6, 6 },
                    { 16, 4, 7 },
                    { 17, 5, 7 },
                    { 18, 6, 7 },
                    { 19, 1, 8 },
                    { 20, 2, 8 },
                    { 21, 3, 8 },
                    { 22, 6, 8 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_cos_line_battery_types_battery_type_id",
                table: "cos_line_battery_types",
                column: "battery_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_cos_line_battery_types_line_id_battery_type_id",
                table: "cos_line_battery_types",
                columns: new[] { "line_id", "battery_type_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cos_line_battery_types");
        }
    }
}
