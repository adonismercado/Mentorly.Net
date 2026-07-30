using Mentorly.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mentorly.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IStudentEnrollmentService, StudentEnrollmentService>();
        services.AddScoped<IPeerReviewService, PeerReviewService>();

        return services;
    }
}
