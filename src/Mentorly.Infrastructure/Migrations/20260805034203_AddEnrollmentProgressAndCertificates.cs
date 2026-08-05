using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mentorly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEnrollmentProgressAndCertificates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "completed_at",
                table: "enrollments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "theme_completions",
                columns: table => new
                {
                    enrollment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    theme_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    completed_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_theme_completions", x => new { x.enrollment_id, x.theme_id });
                    table.ForeignKey(
                        name: "FK_theme_completions_enrollments_enrollment_id",
                        column: x => x.enrollment_id,
                        principalTable: "enrollments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_theme_completions_themes_theme_id",
                        column: x => x.theme_id,
                        principalTable: "themes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "enrollments",
                keyColumn: "id",
                keyValue: new Guid("d9f7ebf1-6f9f-4b61-9870-86ae9be79cb1"),
                column: "completed_at",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_theme_completions_theme_id",
                table: "theme_completions",
                column: "theme_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "theme_completions");

            migrationBuilder.DropColumn(
                name: "completed_at",
                table: "enrollments");
        }
    }
}
