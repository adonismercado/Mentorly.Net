using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class PeerReviewConfiguration : IEntityTypeConfiguration<PeerReview>
{
    public void Configure(EntityTypeBuilder<PeerReview> builder)
    {
        builder.ToTable("peer_reviews");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.SubmissionId)
            .HasColumnName("submission_id")
            .IsRequired();

        builder.Property(x => x.ReviewerStudentId)
            .HasColumnName("reviewer_student_id")
            .IsRequired();

        builder.Property(x => x.IsApproved)
            .HasColumnName("is_approved")
            .IsRequired();

        builder.Property(x => x.FeedbackComment)
            .HasColumnName("feedback_comment")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(x => new { x.SubmissionId, x.ReviewerStudentId })
            .IsUnique();

        builder.HasOne(x => x.ReviewerStudent)
            .WithMany(x => x.PeerReviewsWritten)
            .HasForeignKey(x => x.ReviewerStudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
