using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface ICourseService
{
    Task<CourseDto[]> GetAllCoursesAsync(CancellationToken cancellationToken = default);
    Task<CourseDetailDto?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<CourseContentDto?> GetCourseContentAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<CourseDto?> CreateCourseAsync(Guid adminId, CreateCourseDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateCourseAsync(Guid adminId, Guid courseId, UpdateCourseDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdatePublicationAsync(Guid adminId, Guid courseId, bool isPublished, CancellationToken cancellationToken = default);
    Task<bool> DeleteCourseAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default);
}
