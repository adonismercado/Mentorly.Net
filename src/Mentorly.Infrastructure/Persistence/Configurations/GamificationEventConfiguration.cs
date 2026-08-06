using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class GamificationEventConfiguration : IEntityTypeConfiguration<GamificationEvent>
{
    public void Configure(EntityTypeBuilder<GamificationEvent> builder)
    {
        builder.ToTable("gamification_events"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.StudentId).HasColumnName("student_id");
        builder.Property(x => x.Type).HasColumnName("type").HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(x => x.ReferenceId).HasColumnName("reference_id");
        builder.Property(x => x.Points).HasColumnName("points");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.HasIndex(x => new { x.StudentId, x.Type, x.ReferenceId }).IsUnique();
        builder.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
    }
}
