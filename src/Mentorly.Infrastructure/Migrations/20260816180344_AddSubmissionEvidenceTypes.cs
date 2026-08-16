using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mentorly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionEvidenceTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "evidence_content",
                table: "submissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "evidence_type",
                table: "submissions",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "Url");

            migrationBuilder.Sql("UPDATE submissions SET evidence_content = evidence_url;");

            migrationBuilder.AlterColumn<string>(
                name: "evidence_content",
                table: "submissions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "evidence_url",
                table: "submissions");

            migrationBuilder.CreateIndex(
                name: "IX_submissions_activity_id",
                table: "submissions",
                column: "activity_id");

            migrationBuilder.AddForeignKey(
                name: "FK_submissions_activities_activity_id",
                table: "submissions",
                column: "activity_id",
                principalTable: "activities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_submissions_activities_activity_id",
                table: "submissions");

            migrationBuilder.DropIndex(
                name: "IX_submissions_activity_id",
                table: "submissions");

            migrationBuilder.AddColumn<string>(
                name: "evidence_url",
                table: "submissions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.Sql("UPDATE submissions SET evidence_url = evidence_content;");

            migrationBuilder.AlterColumn<string>(
                name: "evidence_url",
                table: "submissions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "evidence_content",
                table: "submissions");

            migrationBuilder.DropColumn(
                name: "evidence_type",
                table: "submissions");
        }
    }
}
