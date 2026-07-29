using Mentorly.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class StudentRepository(MentorlyDbContext dbContext) : IStudentRepository
{
    public Task<bool> ExistsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return dbContext.Students
            .AnyAsync(x => x.Id == studentId, cancellationToken);
    }
}
