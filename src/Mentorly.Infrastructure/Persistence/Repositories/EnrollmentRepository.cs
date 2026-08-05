using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class EnrollmentRepository(MentorlyDbContext dbContext) : IEnrollmentRepository
{
    public async Task<IReadOnlyList<Enrollment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Enrollments
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Enrollment?> GetByIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        return dbContext.Enrollments
            .Include(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == enrollmentId, cancellationToken);
    }

    public Task<bool> HasActiveEnrollmentAsync(Guid studentId, Guid courseId, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        return dbContext.Enrollments.AnyAsync(x =>
            x.StudentId == studentId &&
            x.CourseId == courseId &&
            x.Status == EnrollmentStatus.Active &&
            x.ExpiresAt > utcNow,
            cancellationToken);
    }

    public async Task<int> GetNextAttemptNumberAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var lastAttempt = await dbContext.Enrollments
            .Where(x => x.StudentId == studentId && x.CourseId == courseId)
            .Select(x => (int?)x.AttemptNumber)
            .MaxAsync(cancellationToken);

        return (lastAttempt ?? 0) + 1;
    }

    public Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        return dbContext.Enrollments.AddAsync(enrollment, cancellationToken).AsTask();
    }

    public void Add(Enrollment enrollment)
    {
        dbContext.Enrollments.Add(enrollment);
    }
}
