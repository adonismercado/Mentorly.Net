using Mentorly.Domain.Entities;

namespace Mentorly.Application.Abstractions.Persistence;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
}
