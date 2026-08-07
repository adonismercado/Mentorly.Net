using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class StudentBadgeConfiguration : IEntityTypeConfiguration<StudentBadge>
{
    public void Configure(EntityTypeBuilder<StudentBadge> builder)
    {
        builder.ToTable("student_badges");

        builder.HasKey(x => new { x.StudentId, x.BadgeId });

        builder.Property(x => x.StudentId).HasColumnName("student_id");
        builder.Property(x => x.BadgeId).HasColumnName("badge_id");
        builder.Property(x => x.GrantedAt).HasColumnName("granted_at").IsRequired();

        builder.HasOne(x => x.Badge)
            .WithMany(x => x.StudentBadges)
            .HasForeignKey(x => x.BadgeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
