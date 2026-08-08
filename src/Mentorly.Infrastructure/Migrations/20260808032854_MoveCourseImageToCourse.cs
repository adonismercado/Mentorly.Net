using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mentorly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveCourseImageToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "courses",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "courses",
                keyColumn: "id",
                keyValue: new Guid("cb57a2a9-aa8e-4538-aa86-d8e383136fdc"),
                column: "image_url",
                value: "https://images.example.com/blazor-fundamentals.png");

            migrationBuilder.DropTable(
                name: "course_images");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "course_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    course_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    alt_text = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
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

            migrationBuilder.InsertData(
                table: "course_images",
                columns: new[] { "id", "alt_text", "course_id", "image_url", "is_cover", "order_index" },
                values: new object[] { new Guid("f74e10ed-86b4-47e5-8caf-d07af6cd2b25"), "Blazor Fundamentals course cover", new Guid("cb57a2a9-aa8e-4538-aa86-d8e383136fdc"), "https://images.example.com/blazor-fundamentals.png", true, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_course_images_course_id_order_index",
                table: "course_images",
                columns: new[] { "course_id", "order_index" },
                unique: true);

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "courses");
        }
    }
}
