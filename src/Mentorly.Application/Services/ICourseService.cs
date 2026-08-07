using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface ICourseService
{
    Task<CourseDto[]> GetAllCoursesAsync(CancellationToken cancellationToken = default);
    Task<CourseDto?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateCourseAsync(Guid courseId, UpdateCourseDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
}
