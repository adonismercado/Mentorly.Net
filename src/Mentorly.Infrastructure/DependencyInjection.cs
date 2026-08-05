using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.Abstractions.Identity;
using Mentorly.Infrastructure.Identity;
using Mentorly.Infrastructure.Persistence;
using Mentorly.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mentorly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MentorlyDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<ISubmissionRepository, SubmissionRepository>();
        services.AddScoped<IPeerReviewRepository, PeerReviewRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ICourseImageRepository, CourseImageRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<IThemeRepository, ThemeRepository>();
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IThemeCompletionRepository, ThemeCompletionRepository>();
        services.AddScoped<IEnrollmentProgressRepository, EnrollmentProgressRepository>();
        services.AddScoped<IPeerReviewWorkflowRepository, PeerReviewWorkflowRepository>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<MentorlyDbContext>());
        services.AddScoped<IStudentIdentityMapper, StudentIdentityMapper>();

        return services;
    }
}
