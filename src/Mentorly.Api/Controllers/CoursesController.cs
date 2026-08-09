using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CourseDto[]>> GetCoursesAsync(CancellationToken cancellationToken = default)
    {
        var courses = await courseService.GetAllCoursesAsync(cancellationToken);
        return Ok(courses);
    }

    [HttpGet("{courseId:guid}")]
    public async Task<ActionResult<CourseDetailDto>> GetCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await courseService.GetCourseByIdAsync(courseId, cancellationToken);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpGet("{courseId:guid}/content")]
    public async Task<ActionResult<CourseContentDto>> GetCourseContentAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var content = await courseService.GetCourseContentAsync(courseId, cancellationToken);
        return content is null ? NotFound() : Ok(content);
    }

    [HttpPost("/api/admins/{adminId:guid}/courses")]
    public async Task<ActionResult<CourseDto>> CreateCourseAsync(Guid adminId, CreateCourseDto dto, CancellationToken cancellationToken = default)
    {
        var course = await courseService.CreateCourseAsync(adminId, dto, cancellationToken);
        return course is null
            ? NotFound()
            : CreatedAtAction(nameof(GetCourseAsync), new { courseId = course.Id }, course);
    }

    [HttpPut("/api/admins/{adminId:guid}/courses/{courseId:guid}")]
    public async Task<IActionResult> UpdateCourseAsync(Guid adminId, Guid courseId, UpdateCourseDto dto, CancellationToken cancellationToken = default)
    {
        return await courseService.UpdateCourseAsync(adminId, courseId, dto, cancellationToken)
            ? NoContent()
            : NotFound();
    }

    [HttpPatch("/api/admins/{adminId:guid}/courses/{courseId:guid}/publication")]
    public async Task<IActionResult> UpdatePublicationAsync(Guid adminId, Guid courseId, UpdateCoursePublicationDto dto, CancellationToken cancellationToken = default)
    {
        return await courseService.UpdatePublicationAsync(adminId, courseId, dto.IsPublished, cancellationToken)
            ? NoContent()
            : NotFound();
    }

    [HttpDelete("/api/admins/{adminId:guid}/courses/{courseId:guid}")]
    public async Task<IActionResult> DeleteCourseAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await courseService.DeleteCourseAsync(adminId, courseId, cancellationToken)
            ? NoContent()
            : NotFound();
    }
}
