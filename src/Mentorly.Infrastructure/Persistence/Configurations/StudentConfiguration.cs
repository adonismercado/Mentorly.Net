using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("students");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.GoogleUserId)
            .HasColumnName("google_user_id")
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.DisplayName)
            .HasColumnName("display_name")
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasMaxLength(32)
            .HasDefaultValue(StudentRole.Student)
            .IsRequired();

        builder.Property(x => x.IsLeaderboardPublic)
            .HasColumnName("is_leaderboard_public")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.TotalPoints)
            .HasColumnName("total_points")
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasIndex(x => x.GoogleUserId)
            .IsUnique();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.HasMany(x => x.StudentBadges)
            .WithOne(x => x.Student)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
