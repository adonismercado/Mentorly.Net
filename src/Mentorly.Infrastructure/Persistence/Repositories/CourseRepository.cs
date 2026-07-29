using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class CourseRepository(MentorlyDbContext dbContext) : ICourseRepository
{
    public Task<Course?> GetByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return dbContext.Courses
            .FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);
    }
}
