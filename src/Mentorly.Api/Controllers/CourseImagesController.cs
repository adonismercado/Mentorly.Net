using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Mentorly.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/courses/{courseId:guid}/images")]
public class CourseImagesController(ICourseImageService courseImageService) : ControllerBase
{
    [Authorize(Policy = MentorlyPolicies.Student)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseImageDto>>> GetAsync(Guid courseId, CancellationToken cancellationToken = default) => Ok(await courseImageService.GetByCourseAsync(courseId, cancellationToken));

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpPost]
    public async Task<ActionResult<CourseImageDto>> CreateAsync(Guid courseId, CreateCourseImageDto dto, CancellationToken cancellationToken = default)
    { var image = await courseImageService.CreateAsync(courseId, dto, cancellationToken); return image is null ? NotFound() : CreatedAtAction(nameof(GetAsync), new { courseId }, image); }

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpPut("{imageId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid courseId, Guid imageId, UpdateCourseImageDto dto, CancellationToken cancellationToken = default) => await courseImageService.UpdateAsync(courseId, imageId, dto, cancellationToken) ? NoContent() : NotFound();

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpDelete("{imageId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid courseId, Guid imageId, CancellationToken cancellationToken = default) => await courseImageService.DeleteAsync(courseId, imageId, cancellationToken) ? NoContent() : NotFound();
}
