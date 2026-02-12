using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "has_ng",
                table: "cos_submissions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "kasie_memo",
                table: "cos_submissions",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "kasie_signed_at",
                table: "cos_submissions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kasubsie_memo",
                table: "cos_submissions",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "kasubsie_signed_at",
                table: "cos_submissions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "leader_memo",
                table: "cos_submissions",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "leader_signed_at",
                table: "cos_submissions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "operator_signed_at",
                table: "cos_submissions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "cos_submissions",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "signed_at",
                table: "cos_signature_entries",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "has_ng",
                table: "cos_submissions");

            migrationBuilder.DropColumn(
                name: "kasie_memo",
                table: "cos_submissions");

            migrationBuilder.DropColumn(
                name: "kasie_signed_at",
                table: "cos_submissions");

            migrationBuilder.DropColumn(
                name: "kasubsie_memo",
                table: "cos_submissions");

            migrationBuilder.DropColumn(
                name: "kasubsie_signed_at",
                table: "cos_submissions");

            migrationBuilder.DropColumn(
                name: "leader_memo",
                table: "cos_submissions");

            migrationBuilder.DropColumn(
                name: "leader_signed_at",
                table: "cos_submissions");

            migrationBuilder.DropColumn(
                name: "operator_signed_at",
                table: "cos_submissions");

            migrationBuilder.DropColumn(
                name: "status",
                table: "cos_submissions");

            migrationBuilder.DropColumn(
                name: "signed_at",
                table: "cos_signature_entries");
        }
    }
}
