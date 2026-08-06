using Mentorly.Domain.Entities;
using Mentorly.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("units"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CourseId).HasColumnName("course_id").IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(x => x.OrderIndex).HasColumnName("order_index").IsRequired();
        builder.HasIndex(x => new { x.CourseId, x.OrderIndex });
        builder.HasMany(x => x.Themes).WithOne(x => x.Unit).HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
        builder.HasData(new { Id = SeedData.UnitId, CourseId = SeedData.CourseId, Title = "Unit 1: Fundamentals", OrderIndex = 1 });
    }
}
