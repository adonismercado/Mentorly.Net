using Mentorly.Domain.Entities;
using Mentorly.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class BadgeConfiguration : IEntityTypeConfiguration<Badge>
{
    public void Configure(EntityTypeBuilder<Badge> builder)
    {
        builder.ToTable("badges");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(128).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
        builder.Property(x => x.ImageUrl).HasColumnName("image_url").HasMaxLength(500);

        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasData(
            new { Id = SeedData.ExplorerBadgeId, Name = "Explorer", Description = "Completed the first theme.", ImageUrl = (string?)null },
            new { Id = SeedData.BuilderBadgeId, Name = "Builder", Description = "Approved the first exercise.", ImageUrl = (string?)null },
            new { Id = SeedData.CollaboratorBadgeId, Name = "Collaborator", Description = "Completed a constructive peer review.", ImageUrl = (string?)null });
    }
}
