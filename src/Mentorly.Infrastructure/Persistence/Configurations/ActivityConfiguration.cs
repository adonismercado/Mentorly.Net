using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;
using Mentorly.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("activities"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ThemeId).HasColumnName("theme_id").IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.IsMandatory).HasColumnName("is_mandatory").IsRequired();
        builder.Property(x => x.ApprovalStrategy).HasColumnName("approval_strategy").HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.OrderIndex).HasColumnName("order_index").IsRequired();
        builder.HasIndex(x => new { x.ThemeId, x.OrderIndex });
        builder.HasData(
            new { Id = SeedData.ActivityId, ThemeId = SeedData.ThemeId, Title = "Build a component", Type = ActivityType.Exercise, IsMandatory = true, ApprovalStrategy = ApprovalStrategy.PeerReview, OrderIndex = 1 },
            new { Id = SeedData.QuizActivityId, ThemeId = SeedData.ThemeId, Title = "Fundamentals quiz", Type = ActivityType.Quiz, IsMandatory = true, ApprovalStrategy = ApprovalStrategy.Auto, OrderIndex = 2 });
    }
}
