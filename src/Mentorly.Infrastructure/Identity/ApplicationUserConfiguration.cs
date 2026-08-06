using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mentorly.Infrastructure.Identity;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.StudentId)
            .HasColumnName("student_id");

        builder.HasIndex(x => x.StudentId)
            .IsUnique()
            .HasFilter("[student_id] IS NOT NULL");
    }
}
