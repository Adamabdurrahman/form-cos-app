using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceItemNumToBatteryType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "source_item_num",
                table: "cos_battery_types",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "cos_battery_types",
                keyColumn: "id",
                keyValue: 1,
                column: "source_item_num",
                value: null);

            migrationBuilder.UpdateData(
                table: "cos_battery_types",
                keyColumn: "id",
                keyValue: 2,
                column: "source_item_num",
                value: null);

            migrationBuilder.UpdateData(
                table: "cos_battery_types",
                keyColumn: "id",
                keyValue: 3,
                column: "source_item_num",
                value: null);

            migrationBuilder.UpdateData(
                table: "cos_battery_types",
                keyColumn: "id",
                keyValue: 4,
                column: "source_item_num",
                value: null);

            migrationBuilder.UpdateData(
                table: "cos_battery_types",
                keyColumn: "id",
                keyValue: 5,
                column: "source_item_num",
                value: null);

            migrationBuilder.UpdateData(
                table: "cos_battery_types",
                keyColumn: "id",
                keyValue: 6,
                column: "source_item_num",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "source_item_num",
                table: "cos_battery_types");
        }
    }
}
