using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionStatusAndLeaderMemo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "has_ng",
                table: "cos_submissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "kasie_signed_at",
                table: "cos_submissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "kasubsie_signed_at",
                table: "cos_submissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "leader_approval_type",
                table: "cos_submissions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "leader_memo",
                table: "cos_submissions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "leader_signed_at",
                table: "cos_submissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "operator_signed_at",
                table: "cos_submissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "cos_submissions",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "signed_at",
                table: "cos_signature_entries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "cos_approval_attachments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    submission_id = table.Column<int>(type: "int", nullable: false),
                    role_key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    file_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    file_path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    content_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    uploaded_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cos_approval_attachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_cos_approval_attachments_cos_submissions_submission_id",
                        column: x => x.submission_id,
                        principalTable: "cos_submissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cos_approval_attachments_submission_id",
                table: "cos_approval_attachments",
                column: "submission_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cos_approval_attachments");

            migrationBuilder.DropColumn(
                name: "has_ng",
                table: "cos_submissions");

            migrationBuilder.DropColumn(
                name: "kasie_signed_at",
                table: "cos_submissions");

            migrationBuilder.DropColumn(
                name: "kasubsie_signed_at",
                table: "cos_submissions");

            migrationBuilder.DropColumn(
                name: "leader_approval_type",
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
