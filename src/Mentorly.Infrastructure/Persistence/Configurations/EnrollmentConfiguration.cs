using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;
using Mentorly.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("enrollments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.StudentId)
            .HasColumnName("student_id")
            .IsRequired();

        builder.Property(x => x.CourseId)
            .HasColumnName("course_id")
            .IsRequired();

        builder.Property(x => x.AttemptNumber)
            .HasColumnName("attempt_number")
            .IsRequired();

        builder.Property(x => x.StartedAt)
            .HasColumnName("started_at")
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(32)
            .HasDefaultValue(EnrollmentStatus.Active)
            .IsRequired();

        builder.Property(x => x.CertificateUrl)
            .HasColumnName("certificate_url")
            .HasMaxLength(500);

        builder.HasIndex(x => new { x.StudentId, x.CourseId, x.AttemptNumber })
            .IsUnique();

        builder.HasOne(x => x.Student)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Submissions)
            .WithOne(x => x.Enrollment)
            .HasForeignKey(x => x.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(new
        {
            Id = SeedData.SeedEnrollmentId,
            StudentId = SeedData.ReviewerStudentId,
            CourseId = SeedData.CourseId,
            AttemptNumber = 1,
            StartedAt = SeedData.SeedStartedAtUtc,
            ExpiresAt = SeedData.SeedStartedAtUtc.AddMonths(3),
            Status = EnrollmentStatus.Active,
            CertificateUrl = (string?)null
        });
    }
}
