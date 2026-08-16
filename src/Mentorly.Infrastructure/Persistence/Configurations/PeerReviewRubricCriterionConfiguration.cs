using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class PeerReviewRubricCriterionConfiguration : IEntityTypeConfiguration<PeerReviewRubricCriterion>
{
    public void Configure(EntityTypeBuilder<PeerReviewRubricCriterion> builder)
    {
        builder.ToTable("peer_review_rubric_criteria"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id"); builder.Property(x => x.ActivityId).HasColumnName("activity_id").IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired(); builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.MaxScore).HasColumnName("max_score").IsRequired(); builder.Property(x => x.OrderIndex).HasColumnName("order_index").IsRequired();
        builder.HasIndex(x => new { x.ActivityId, x.OrderIndex }).IsUnique();
        builder.HasOne(x => x.Activity).WithMany(x => x.PeerReviewRubricCriteria).HasForeignKey(x => x.ActivityId).OnDelete(DeleteBehavior.Cascade);
    }
}
