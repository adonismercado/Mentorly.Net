using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class PeerReviewCriterionScoreConfiguration : IEntityTypeConfiguration<PeerReviewCriterionScore>
{
    public void Configure(EntityTypeBuilder<PeerReviewCriterionScore> builder)
    {
        builder.ToTable("peer_review_criterion_scores"); builder.HasKey(x => new { x.PeerReviewId, x.RubricCriterionId });
        builder.Property(x => x.PeerReviewId).HasColumnName("peer_review_id"); builder.Property(x => x.RubricCriterionId).HasColumnName("rubric_criterion_id"); builder.Property(x => x.Score).HasColumnName("score").IsRequired();
        builder.HasOne(x => x.PeerReview).WithMany(x => x.CriterionScores).HasForeignKey(x => x.PeerReviewId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.RubricCriterion).WithMany().HasForeignKey(x => x.RubricCriterionId).OnDelete(DeleteBehavior.Restrict);
    }
}
