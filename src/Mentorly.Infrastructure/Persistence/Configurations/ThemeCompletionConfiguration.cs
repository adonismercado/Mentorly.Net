using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class ThemeCompletionConfiguration : IEntityTypeConfiguration<ThemeCompletion>
{
    public void Configure(EntityTypeBuilder<ThemeCompletion> builder)
    {
        builder.ToTable("theme_completions");
        builder.HasKey(x => new { x.EnrollmentId, x.ThemeId });
        builder.Property(x => x.EnrollmentId).HasColumnName("enrollment_id");
        builder.Property(x => x.ThemeId).HasColumnName("theme_id");
        builder.Property(x => x.CompletedAt).HasColumnName("completed_at").IsRequired();
        builder.HasOne(x => x.Theme).WithMany().HasForeignKey(x => x.ThemeId).OnDelete(DeleteBehavior.Restrict);
    }
}
