using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class CourseRepository(MentorlyDbContext dbContext) : ICourseRepository
{
    public Task<Course[]> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Courses
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);
    }

    public Task<Course?> GetByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return dbContext.Courses
            .FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);
    }

    public void Add(Course course)
    {
        dbContext.Courses.Add(course);
    }

    public void Update(Course course)
    {
        dbContext.Courses.Update(course);
    }

    public void Delete(Course course)
    {
        dbContext.Courses.Remove(course);
    }
}
