using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mentorly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    student_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

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
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.InsertData(
                table: "courses",
                columns: new[] { "id", "created_at", "created_by_admin_id", "description", "is_published", "required_peer_reviews", "title" },
                values: new object[] { new Guid("cb57a2a9-aa8e-4538-aa86-d8e383136fdc"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("80bbec34-8a28-4e38-ab64-92662f0b5b5b"), "Seed course for clean architecture demo.", true, 1, "Blazor Fundamentals" });

            migrationBuilder.InsertData(
                table: "students",
                columns: new[] { "id", "display_name", "email", "google_user_id", "is_leaderboard_public", "role" },
                values: new object[,]
                {
                    { new Guid("b7e670c1-caf3-4da5-a8f7-34570fbb9d41"), "Student Two", "student2@mentorly.local", "google-student-002", true, "Student" },
                    { new Guid("f43f2c2f-2db4-47cd-8a42-7b0f3c495601"), "Student One", "student1@mentorly.local", "google-student-001", true, "Student" }
                });

            migrationBuilder.InsertData(
                table: "course_images",
                columns: new[] { "id", "alt_text", "course_id", "image_url", "is_cover", "order_index" },
                values: new object[] { new Guid("f74e10ed-86b4-47e5-8caf-d07af6cd2b25"), "Blazor Fundamentals course cover", new Guid("cb57a2a9-aa8e-4538-aa86-d8e383136fdc"), "https://images.example.com/blazor-fundamentals.png", true, 1 });

            migrationBuilder.InsertData(
                table: "enrollments",
                columns: new[] { "id", "attempt_number", "certificate_url", "completed_at", "course_id", "expires_at", "started_at", "status", "student_id" },
                values: new object[,]
                {
                    { new Guid("b82acd0a-9bd4-4e5e-b2d9-01e3283285f1"), 1, null, null, new Guid("cb57a2a9-aa8e-4538-aa86-d8e383136fdc"), new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Active", new Guid("f43f2c2f-2db4-47cd-8a42-7b0f3c495601") },
                    { new Guid("d9f7ebf1-6f9f-4b61-9870-86ae9be79cb1"), 1, null, null, new Guid("cb57a2a9-aa8e-4538-aa86-d8e383136fdc"), new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Active", new Guid("b7e670c1-caf3-4da5-a8f7-34570fbb9d41") }
                });

            migrationBuilder.InsertData(
                table: "units",
                columns: new[] { "id", "course_id", "order_index", "title" },
                values: new object[] { new Guid("be480fd4-6392-4a0d-91fd-5a3e773e9c10"), new Guid("cb57a2a9-aa8e-4538-aa86-d8e383136fdc"), 1, "Unit 1: Fundamentals" });

            migrationBuilder.InsertData(
                table: "submissions",
                columns: new[] { "id", "activity_id", "enrollment_id", "evidence_url", "reviewed_at", "status", "submitted_at" },
                values: new object[,]
                {
                    { new Guid("9980b9e0-d0cc-42f5-bf54-e5f3fd56bc56"), new Guid("f3af6a42-266d-4468-b840-f26e95ec6e6b"), new Guid("d9f7ebf1-6f9f-4b61-9870-86ae9be79cb1"), "https://github.com/example/reviewer-seed", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Approved", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a1904ac6-c334-4126-9f2f-03dd9a6276e6"), new Guid("f3af6a42-266d-4468-b840-f26e95ec6e6b"), new Guid("b82acd0a-9bd4-4e5e-b2d9-01e3283285f1"), "https://github.com/example/author-seed", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Approved", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "themes",
                columns: new[] { "id", "content_text", "order_index", "title", "unit_id" },
                values: new object[] { new Guid("a8466ce6-95c6-4d7d-a998-38925240cd70"), "Introduction to components, parameters, and state.", 1, "Components and state", new Guid("be480fd4-6392-4a0d-91fd-5a3e773e9c10") });

            migrationBuilder.InsertData(
                table: "activities",
                columns: new[] { "id", "approval_strategy", "is_mandatory", "order_index", "theme_id", "title", "type" },
                values: new object[,]
                {
                    { new Guid("6a6538ef-9454-4a5d-80ac-344d8a4068de"), "Auto", true, 2, new Guid("a8466ce6-95c6-4d7d-a998-38925240cd70"), "Fundamentals quiz", "Quiz" },
                    { new Guid("f3af6a42-266d-4468-b840-f26e95ec6e6b"), "PeerReview", true, 1, new Guid("a8466ce6-95c6-4d7d-a998-38925240cd70"), "Build a component", "Exercise" }
                });

            migrationBuilder.InsertData(
                table: "peer_reviews",
                columns: new[] { "id", "created_at", "feedback_comment", "is_approved", "reviewer_student_id", "submission_id" },
                values: new object[] { new Guid("1f3c9c12-c628-4d29-9887-271c4cd71fe0"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), "The component structure is clear and the state handling is correct.", true, new Guid("b7e670c1-caf3-4da5-a8f7-34570fbb9d41"), new Guid("a1904ac6-c334-4126-9f2f-03dd9a6276e6") });

            migrationBuilder.CreateIndex(
                name: "IX_activities_theme_id_order_index",
                table: "activities",
                columns: new[] { "theme_id", "order_index" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_student_id",
                table: "AspNetUsers",
                column: "student_id",
                unique: true,
                filter: "[student_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_badges_name",
                table: "badges",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_images_course_id_order_index",
                table: "course_images",
                columns: new[] { "course_id", "order_index" },
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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "course_images");

            migrationBuilder.DropTable(
                name: "gamification_events");

            migrationBuilder.DropTable(
                name: "peer_reviews");

            migrationBuilder.DropTable(
                name: "student_badges");

            migrationBuilder.DropTable(
                name: "theme_completions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

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
