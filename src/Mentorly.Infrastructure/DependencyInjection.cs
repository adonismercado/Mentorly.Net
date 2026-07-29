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
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string sqliteConnectionString)
    {
        services.AddDbContext<MentorlyDbContext>(options =>
            options.UseSqlite(sqliteConnectionString));

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<ISubmissionRepository, SubmissionRepository>();
        services.AddScoped<IPeerReviewRepository, PeerReviewRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<MentorlyDbContext>());
        services.AddScoped<IStudentIdentityMapper, StudentIdentityMapper>();

        return services;
    }
}
