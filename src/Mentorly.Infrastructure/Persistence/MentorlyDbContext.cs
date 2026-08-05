using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Mentorly.Infrastructure.Identity;
using Mentorly.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence;

public sealed class MentorlyDbContext(
    DbContextOptions<MentorlyDbContext> options
) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options), IUnitOfWork
{
    public DbSet<Student> Students => Set<Student>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    public DbSet<Submission> Submissions => Set<Submission>();

    public DbSet<PeerReview> PeerReviews => Set<PeerReview>();

    public DbSet<Badge> Badges => Set<Badge>();

    public DbSet<StudentBadge> StudentBadges => Set<StudentBadge>();

    public DbSet<CourseImage> CourseImages => Set<CourseImage>();

    public DbSet<Unit> Units => Set<Unit>();

    public DbSet<Theme> Themes => Set<Theme>();

    public DbSet<Activity> Activities => Set<Activity>();

    public DbSet<ThemeCompletion> ThemeCompletions => Set<ThemeCompletion>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new SubmissionConfiguration());
        modelBuilder.ApplyConfiguration(new PeerReviewConfiguration());
        modelBuilder.ApplyConfiguration(new BadgeConfiguration());
        modelBuilder.ApplyConfiguration(new StudentBadgeConfiguration());
        modelBuilder.ApplyConfiguration(new CourseImageConfiguration());
        modelBuilder.ApplyConfiguration(new UnitConfiguration());
        modelBuilder.ApplyConfiguration(new ThemeConfiguration());
        modelBuilder.ApplyConfiguration(new ActivityConfiguration());
        modelBuilder.ApplyConfiguration(new ThemeCompletionConfiguration());

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
    }
}
