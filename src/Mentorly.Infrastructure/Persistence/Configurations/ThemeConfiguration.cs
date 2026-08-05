using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class ThemeConfiguration : IEntityTypeConfiguration<Theme>
{
    public void Configure(EntityTypeBuilder<Theme> builder)
    {
        builder.ToTable("themes"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(x => x.ContentText).HasColumnName("content_text").HasMaxLength(20000).IsRequired();
        builder.Property(x => x.OrderIndex).HasColumnName("order_index").IsRequired();
        builder.HasIndex(x => new { x.UnitId, x.OrderIndex });
        builder.HasMany(x => x.Activities).WithOne(x => x.Theme).HasForeignKey(x => x.ThemeId).OnDelete(DeleteBehavior.Restrict);
    }
}
