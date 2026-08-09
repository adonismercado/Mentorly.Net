using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mentorly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "courses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    created_by_admin_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_published = table.Column<bool>(type: "bit", nullable: false),
                    required_peer_reviews = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "quiz_attempts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    enrollment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    activity_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    passed = table.Column<bool>(type: "bit", nullable: false),
                    submitted_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_attempts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "quiz_questions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    activity_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    prompt = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    correct_answer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    order_index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_questions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "students",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    google_user_id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    display_name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    role = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, defaultValue: "Student"),
                    is_leaderboard_public = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    total_points = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students", x => x.id);
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
                name: "enrollments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    student_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    course_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    attempt_number = table.Column<int>(type: "int", nullable: false),
                    started_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, defaultValue: "Active"),
                    certificate_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    completed_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enrollments", x => x.id);
                    table.ForeignKey(
                        name: "FK_enrollments_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_enrollments_students_student_id",
                        column: x => x.student_id,
                        principalTable: "students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gamification_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    student_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    type = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    reference_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    points = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gamification_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_gamification_events_students_student_id",
                        column: x => x.student_id,
                        principalTable: "students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "submissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    enrollment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    activity_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    evidence_url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, defaultValue: "Pending"),
                    submitted_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    reviewed_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_submissions_enrollments_enrollment_id",
                        column: x => x.enrollment_id,
                        principalTable: "enrollments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "peer_reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    submission_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    reviewer_student_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_approved = table.Column<bool>(type: "bit", nullable: false),
                    feedback_comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_peer_reviews", x => x.id);
                    table.ForeignKey(
                        name: "FK_peer_reviews_students_reviewer_student_id",
                        column: x => x.reviewer_student_id,
                        principalTable: "students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_peer_reviews_submissions_submission_id",
                        column: x => x.submission_id,
                        principalTable: "submissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activities_theme_id_order_index",
                table: "activities",
                columns: new[] { "theme_id", "order_index" });

            migrationBuilder.CreateIndex(
                name: "IX_badges_name",
                table: "badges",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_enrollments_course_id",
                table: "enrollments",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_enrollments_student_id_course_id_attempt_number",
                table: "enrollments",
                columns: new[] { "student_id", "course_id", "attempt_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gamification_events_student_id_type_reference_id",
                table: "gamification_events",
                columns: new[] { "student_id", "type", "reference_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_peer_reviews_reviewer_student_id",
                table: "peer_reviews",
                column: "reviewer_student_id");

            migrationBuilder.CreateIndex(
                name: "IX_peer_reviews_submission_id_reviewer_student_id",
                table: "peer_reviews",
                columns: new[] { "submission_id", "reviewer_student_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quiz_attempts_enrollment_id_activity_id",
                table: "quiz_attempts",
                columns: new[] { "enrollment_id", "activity_id" });

            migrationBuilder.CreateIndex(
                name: "IX_quiz_questions_activity_id_order_index",
                table: "quiz_questions",
                columns: new[] { "activity_id", "order_index" });

            migrationBuilder.CreateIndex(
                name: "IX_student_badges_badge_id",
                table: "student_badges",
                column: "badge_id");

            migrationBuilder.CreateIndex(
                name: "IX_students_email",
                table: "students",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_students_google_user_id",
                table: "students",
                column: "google_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_submissions_enrollment_id_activity_id",
                table: "submissions",
                columns: new[] { "enrollment_id", "activity_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_theme_completions_theme_id",
                table: "theme_completions",
                column: "theme_id");

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
                name: "gamification_events");

            migrationBuilder.DropTable(
                name: "peer_reviews");

            migrationBuilder.DropTable(
                name: "quiz_attempts");

            migrationBuilder.DropTable(
                name: "quiz_questions");

            migrationBuilder.DropTable(
                name: "student_badges");

            migrationBuilder.DropTable(
                name: "theme_completions");

            migrationBuilder.DropTable(
                name: "submissions");

            migrationBuilder.DropTable(
                name: "badges");

            migrationBuilder.DropTable(
                name: "themes");

            migrationBuilder.DropTable(
                name: "enrollments");

            migrationBuilder.DropTable(
                name: "units");

            migrationBuilder.DropTable(
                name: "students");

            migrationBuilder.DropTable(
                name: "courses");
        }
    }
}
