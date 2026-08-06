using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class StudentRepository(MentorlyDbContext dbContext) : IStudentRepository
{
    public Task<bool> ExistsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return dbContext.Students
            .AnyAsync(x => x.Id == studentId, cancellationToken);
    }

    public async Task<Student?> GetByIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Students
            .FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken);
    }

    public async Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Students
            .ToListAsync(cancellationToken);
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