using Mentorly.Domain.Entities;

namespace Mentorly.Application.Abstractions.Persistence;

public interface ICourseRepository
{
    Task<Course[]> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Course?> GetByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    void Add(Course course);
    void Update(Course course);
    void Delete(Course course);
}
