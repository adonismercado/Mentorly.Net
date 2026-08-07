using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Entities;

namespace Mentorly.Application.Services;

public sealed class CourseService(
    ICourseRepository courseRepository,
    IUnitOfWork unitOfWork) : ICourseService
{
    public async Task<CourseDto[]> GetAllCoursesAsync(CancellationToken cancellationToken = default)
    {
        var courses = await courseRepository.GetAllAsync(cancellationToken);

        return courses.Select(c => new CourseDto(
            c.Id,
            c.Title,
            c.Description,
            c.CreatedByAdminId,
            c.IsPublished,
            c.RequiredPeerReviews,
            c.CreatedAt))
            .ToArray();
    }

    public async Task<CourseDto?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);

        if (course is null)
        {
            return null;
        }

        return new CourseDto(
            course.Id,
            course.Title,
            course.Description,
            course.CreatedByAdminId,
            course.IsPublished,
            course.RequiredPeerReviews,
            course.CreatedAt);
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, CancellationToken cancellationToken = default)
    {
        var course = new Course(
            Guid.NewGuid(),
            dto.Title,
            dto.Description,
            dto.CreatedByAdminId,
            dto.RequiredPeerReviews);

        courseRepository.Add(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CourseDto(
            course.Id,
            course.Title,
            course.Description,
            course.CreatedByAdminId,
            course.IsPublished,
            course.RequiredPeerReviews,
            course.CreatedAt);
    }

    public async Task<bool> UpdateCourseAsync(Guid courseId, UpdateCourseDto dto, CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);

        if (course is null)
        {
            return false;
        }

        course.Rename(dto.Title);
        course.UpdateDescription(dto.Description);
        course.UpdateRequiredPeerReviews(dto.RequiredPeerReviews);

        courseRepository.Update(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);

        if (course is null)
        {
            return false;
        }

        courseRepository.Delete(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
