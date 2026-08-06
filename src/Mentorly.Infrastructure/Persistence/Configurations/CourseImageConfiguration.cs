using Mentorly.Domain.Entities;
using Mentorly.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Persistence.Configurations;

public sealed class CourseImageConfiguration : IEntityTypeConfiguration<CourseImage>
{
    public void Configure(EntityTypeBuilder<CourseImage> builder)
    {
        builder.ToTable("course_images");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CourseId).HasColumnName("course_id").IsRequired();
        builder.Property(x => x.ImageUrl).HasColumnName("image_url").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.AltText).HasColumnName("alt_text").HasMaxLength(250).IsRequired();
        builder.Property(x => x.IsCover).HasColumnName("is_cover").IsRequired();
        builder.Property(x => x.OrderIndex).HasColumnName("order_index").IsRequired();
        builder.HasIndex(x => new { x.CourseId, x.OrderIndex }).IsUnique();
        builder.HasData(new { Id = SeedData.CourseImageId, CourseId = SeedData.CourseId, ImageUrl = "https://images.example.com/blazor-fundamentals.png", AltText = "Blazor Fundamentals course cover", IsCover = true, OrderIndex = 1 });
    }
}
