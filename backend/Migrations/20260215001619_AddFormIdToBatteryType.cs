using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddFormIdToBatteryType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "form_id",
                table: "cos_battery_types",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "cos_battery_types",
                keyColumn: "id",
                keyValue: 1,
                column: "form_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "cos_battery_types",
                keyColumn: "id",
                keyValue: 2,
                column: "form_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "cos_battery_types",
                keyColumn: "id",
                keyValue: 3,
                column: "form_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "cos_battery_types",
                keyColumn: "id",
                keyValue: 4,
                column: "form_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "cos_battery_types",
                keyColumn: "id",
                keyValue: 5,
                column: "form_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "cos_battery_types",
                keyColumn: "id",
                keyValue: 6,
                column: "form_id",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_cos_battery_types_form_id",
                table: "cos_battery_types",
                column: "form_id");

            migrationBuilder.AddForeignKey(
                name: "FK_cos_battery_types_cos_form_definitions_form_id",
                table: "cos_battery_types",
                column: "form_id",
                principalTable: "cos_form_definitions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cos_battery_types_cos_form_definitions_form_id",
                table: "cos_battery_types");

            migrationBuilder.DropIndex(
                name: "IX_cos_battery_types_form_id",
                table: "cos_battery_types");

            migrationBuilder.DropColumn(
                name: "form_id",
                table: "cos_battery_types");
        }
    }
}
