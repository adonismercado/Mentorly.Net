using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mentorly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityStudentProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_leaderboard_public",
                table: "students",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "students",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Student");

            migrationBuilder.AddColumn<int>(
                name: "total_points",
                table: "students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "student_id",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "badges",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badges", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "student_badges",
                columns: table => new
                {
                    student_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    badge_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    granted_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_badges", x => new { x.student_id, x.badge_id });
                    table.ForeignKey(
                        name: "FK_student_badges_badges_badge_id",
                        column: x => x.badge_id,
                        principalTable: "badges",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_badges_students_student_id",
                        column: x => x.student_id,
                        principalTable: "students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "students",
                keyColumn: "id",
                keyValue: new Guid("b7e670c1-caf3-4da5-a8f7-34570fbb9d41"),
                columns: new[] { "is_leaderboard_public", "role" },
                values: new object[] { true, "Student" });

            migrationBuilder.UpdateData(
                table: "students",
                keyColumn: "id",
                keyValue: new Guid("f43f2c2f-2db4-47cd-8a42-7b0f3c495601"),
                columns: new[] { "is_leaderboard_public", "role" },
                values: new object[] { true, "Student" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_student_id",
                table: "AspNetUsers",
                column: "student_id",
                unique: true,
                filter: "[student_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_badges_name",
                table: "badges",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_badges_badge_id",
                table: "student_badges",
                column: "badge_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "student_badges");

            migrationBuilder.DropTable(
                name: "badges");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_student_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "is_leaderboard_public",
                table: "students");

            migrationBuilder.DropColumn(
                name: "role",
                table: "students");

            migrationBuilder.DropColumn(
                name: "total_points",
                table: "students");

            migrationBuilder.DropColumn(
                name: "student_id",
                table: "AspNetUsers");
        }
    }
}
