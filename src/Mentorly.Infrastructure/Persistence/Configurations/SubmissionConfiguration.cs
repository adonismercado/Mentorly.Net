using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("submissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.EnrollmentId)
            .HasColumnName("enrollment_id")
            .IsRequired();

        builder.Property(x => x.ActivityId)
            .HasColumnName("activity_id")
            .IsRequired();

        builder.Property(x => x.EvidenceUrl)
            .HasColumnName("evidence_url")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(32)
            .HasDefaultValue(SubmissionStatus.Pending)
            .IsRequired();

        builder.Property(x => x.SubmittedAt)
            .HasColumnName("submitted_at")
            .IsRequired();

        builder.Property(x => x.ReviewedAt)
            .HasColumnName("reviewed_at");

        builder.HasIndex(x => new { x.EnrollmentId, x.ActivityId })
            .IsUnique();

        builder.HasMany(x => x.PeerReviews)
            .WithOne(x => x.Submission)
            .HasForeignKey(x => x.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
