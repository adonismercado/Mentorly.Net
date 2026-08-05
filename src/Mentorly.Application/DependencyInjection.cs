using Mentorly.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mentorly.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<IStudentEnrollmentService, StudentEnrollmentService>();
        services.AddScoped<IPeerReviewService, PeerReviewService>();
        services.AddScoped<ICourseImageService, CourseImageService>();
        services.AddScoped<IUnitService, UnitService>();
        services.AddScoped<IThemeService, ThemeService>();
        services.AddScoped<IActivityService, ActivityService>();

        return services;
    }
}
