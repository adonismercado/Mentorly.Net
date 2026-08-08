using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Application.Services;

public sealed class CourseService(
    ICourseRepository courseRepository,
    IStudentRepository studentRepository,
    IUnitRepository unitRepository,
    IThemeRepository themeRepository,
    IActivityRepository activityRepository,
    IUnitOfWork unitOfWork) : ICourseService
{
    public async Task<CourseDto[]> GetAllCoursesAsync(CancellationToken cancellationToken = default)
    {
        var courses = await courseRepository.GetAllAsync(cancellationToken);
        return courses
            .Where(course => course.IsPublished)
            .Select(MapCourse)
            .ToArray();
    }

    public async Task<CourseDetailDto?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null)
        {
            return null;
        }

        var units = await unitRepository.GetByCourseIdAsync(courseId, cancellationToken);
        return new CourseDetailDto(
            course.Id,
            course.Title,
            course.Description,
            course.ImageUrl,
            course.IsPublished,
            course.RequiredPeerReviews,
            units.Select(unit => new UnitDto(unit.Id, unit.CourseId, unit.Title, unit.OrderIndex)).ToArray());
    }

    public async Task<CourseContentDto?> GetCourseContentAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null)
        {
            return null;
        }

        var units = await unitRepository.GetByCourseIdAsync(courseId, cancellationToken);
        var unitDtos = new List<CourseUnitContentDto>();

        foreach (var unit in units)
        {
            var themes = await themeRepository.GetByUnitIdAsync(unit.Id, cancellationToken);
            var themeDtos = new List<CourseThemeContentDto>();

            foreach (var theme in themes)
            {
                var activities = await activityRepository.GetByThemeIdAsync(theme.Id, cancellationToken);
                themeDtos.Add(new CourseThemeContentDto(
                    theme.Id,
                    theme.Title,
                    theme.ContentText,
                    theme.OrderIndex,
                    activities.Select(activity => new CourseActivityContentDto(
                        activity.Id,
                        activity.Title,
                        activity.Type,
                        activity.IsMandatory,
                        activity.ApprovalStrategy,
                        activity.OrderIndex)).ToArray()));
            }

            unitDtos.Add(new CourseUnitContentDto(unit.Id, unit.Title, unit.OrderIndex, themeDtos.ToArray()));
        }

        return new CourseContentDto(
            course.Id,
            course.Title,
            course.Description,
            course.ImageUrl,
            course.IsPublished,
            course.RequiredPeerReviews,
            unitDtos.ToArray());
    }

    public async Task<CourseDto?> CreateCourseAsync(Guid adminId, CreateCourseDto dto, CancellationToken cancellationToken = default)
    {
        if (!await IsAdminAsync(adminId, cancellationToken))
        {
            return null;
        }

        var course = new Course(
            Guid.NewGuid(),
            dto.Title,
            dto.Description,
            adminId,
            dto.RequiredPeerReviews,
            dto.ImageUrl);

        courseRepository.Add(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return MapCourse(course);
    }

    public async Task<bool> UpdateCourseAsync(Guid adminId, Guid courseId, UpdateCourseDto dto, CancellationToken cancellationToken = default)
    {
        if (!await IsAdminAsync(adminId, cancellationToken))
        {
            return false;
        }

        var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null)
        {
            return false;
        }

        course.Rename(dto.Title);
        course.UpdateDescription(dto.Description);
        course.UpdateImageUrl(dto.ImageUrl);
        course.UpdateRequiredPeerReviews(dto.RequiredPeerReviews);

        courseRepository.Update(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdatePublicationAsync(Guid adminId, Guid courseId, bool isPublished, CancellationToken cancellationToken = default)
    {
        if (!await IsAdminAsync(adminId, cancellationToken))
        {
            return false;
        }

        var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null)
        {
            return false;
        }

        if (isPublished)
        {
            course.Publish();
        }
        else
        {
            course.Unpublish();
        }

        courseRepository.Update(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteCourseAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default)
    {
        if (!await IsAdminAsync(adminId, cancellationToken))
        {
            return false;
        }

        var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null)
        {
            return false;
        }

        courseRepository.Delete(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<bool> IsAdminAsync(Guid studentId, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(studentId, cancellationToken);
        return student?.Role == StudentRole.Admin;
    }

    private static CourseDto MapCourse(Course course) => new(
        course.Id,
        course.Title,
        course.Description,
        course.ImageUrl,
        course.IsPublished,
        course.RequiredPeerReviews);
}
