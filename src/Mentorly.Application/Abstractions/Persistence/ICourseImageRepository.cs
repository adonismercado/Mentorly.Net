using Mentorly.Domain.Entities;

namespace Mentorly.Application.Abstractions.Persistence;
public interface ICourseImageRepository { Task<IReadOnlyList<CourseImage>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default); Task<CourseImage?> GetByIdAsync(Guid imageId, CancellationToken cancellationToken = default); void Add(CourseImage image); void Update(CourseImage image); void Delete(CourseImage image); }
