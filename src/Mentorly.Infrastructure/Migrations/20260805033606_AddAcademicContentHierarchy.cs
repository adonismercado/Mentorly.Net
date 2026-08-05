using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mentorly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicContentHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "course_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    course_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    alt_text = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    is_cover = table.Column<bool>(type: "bit", nullable: false),
                    order_index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_course_images_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "units",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    course_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    order_index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_units", x => x.id);
                    table.ForeignKey(
                        name: "FK_units_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "themes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    unit_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    content_text = table.Column<string>(type: "nvarchar(max)", maxLength: 20000, nullable: false),
                    order_index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_themes", x => x.id);
                    table.ForeignKey(
                        name: "FK_themes_units_unit_id",
                        column: x => x.unit_id,
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "activities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    theme_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    type = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    is_mandatory = table.Column<bool>(type: "bit", nullable: false),
                    approval_strategy = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    order_index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activities", x => x.id);
                    table.ForeignKey(
                        name: "FK_activities_themes_theme_id",
                        column: x => x.theme_id,
                        principalTable: "themes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activities_theme_id_order_index",
                table: "activities",
                columns: new[] { "theme_id", "order_index" });

            migrationBuilder.CreateIndex(
                name: "IX_course_images_course_id_order_index",
                table: "course_images",
                columns: new[] { "course_id", "order_index" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_themes_unit_id_order_index",
                table: "themes",
                columns: new[] { "unit_id", "order_index" });

            migrationBuilder.CreateIndex(
                name: "IX_units_course_id_order_index",
                table: "units",
                columns: new[] { "course_id", "order_index" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activities");

            migrationBuilder.DropTable(
                name: "course_images");

            migrationBuilder.DropTable(
                name: "themes");

            migrationBuilder.DropTable(
                name: "units");
        }
    }
}
