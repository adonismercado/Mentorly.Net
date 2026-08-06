using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class StudentRepository(MentorlyDbContext dbContext) : IStudentRepository
{
    public async Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Students
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Student?> GetByIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return dbContext.Students
            .FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken);
    }

    public Task<Student?> GetByIdWithBadgesAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return dbContext.Students
            .Include(x => x.StudentBadges)
            .ThenInclude(x => x.Badge)
            .FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return dbContext.Students
            .AnyAsync(x => x.Id == studentId, cancellationToken);
    }

    public void Add(Student student)
    {
        dbContext.Students.Add(student);
    }

    public void Update(Student student)
    {
        dbContext.Students.Update(student);
    }

    public void Delete(Student student)
    {
        dbContext.Students.Remove(student);
    }
}
