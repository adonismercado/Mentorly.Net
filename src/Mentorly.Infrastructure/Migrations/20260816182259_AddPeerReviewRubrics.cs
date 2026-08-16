using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mentorly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPeerReviewRubrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "peer_review_rubric_criteria",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    activity_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    max_score = table.Column<int>(type: "int", nullable: false),
                    order_index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_peer_review_rubric_criteria", x => x.id);
                    table.ForeignKey(
                        name: "FK_peer_review_rubric_criteria_activities_activity_id",
                        column: x => x.activity_id,
                        principalTable: "activities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "peer_review_criterion_scores",
                columns: table => new
                {
                    peer_review_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    rubric_criterion_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    score = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_peer_review_criterion_scores", x => new { x.peer_review_id, x.rubric_criterion_id });
                    table.ForeignKey(
                        name: "FK_peer_review_criterion_scores_peer_review_rubric_criteria_rubric_criterion_id",
                        column: x => x.rubric_criterion_id,
                        principalTable: "peer_review_rubric_criteria",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_peer_review_criterion_scores_peer_reviews_peer_review_id",
                        column: x => x.peer_review_id,
                        principalTable: "peer_reviews",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_peer_review_criterion_scores_rubric_criterion_id",
                table: "peer_review_criterion_scores",
                column: "rubric_criterion_id");

            migrationBuilder.CreateIndex(
                name: "IX_peer_review_rubric_criteria_activity_id_order_index",
                table: "peer_review_rubric_criteria",
                columns: new[] { "activity_id", "order_index" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "peer_review_criterion_scores");

            migrationBuilder.DropTable(
                name: "peer_review_rubric_criteria");
        }
    }
}
