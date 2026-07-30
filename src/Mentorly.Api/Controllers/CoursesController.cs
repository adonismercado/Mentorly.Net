using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesAsync(CancellationToken cancellationToken = default)
    {
        var courses = await courseService.GetAllCoursesAsync(cancellationToken);
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await courseService.GetCourseByIdAsync(id, cancellationToken);

        if (course is null)
        {
            return NotFound();
        }

        return Ok(course);
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourseAsync(CreateCourseDto dto, CancellationToken cancellationToken = default)
    {
        var course = await courseService.CreateCourseAsync(dto, cancellationToken);
        return CreatedAtAction("GetCourse", new { id = course.Id }, course);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourseAsync(Guid id, UpdateCourseDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await courseService.UpdateCourseAsync(id, dto, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await courseService.DeleteCourseAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
