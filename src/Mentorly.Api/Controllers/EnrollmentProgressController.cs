using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api")]
public class EnrollmentProgressController(IEnrollmentProgressService enrollmentProgressService) : ControllerBase
{
    [HttpGet("enrollments/{enrollmentId:guid}/progress")]
    public async Task<ActionResult<EnrollmentProgressDto>> GetProgressAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var progress = await enrollmentProgressService.GetProgressAsync(enrollmentId, cancellationToken);
        return progress is null ? NotFound() : Ok(progress);
    }

    [HttpPost("enrollments/{enrollmentId:guid}/themes/{themeId:guid}/complete")]
    public async Task<ActionResult<EnrollmentProgressDto>> CompleteThemeAsync(Guid enrollmentId, Guid themeId, CancellationToken cancellationToken = default)
    {
        try
        {
            var progress = await enrollmentProgressService.CompleteThemeAsync(enrollmentId, themeId, cancellationToken);
            return progress is null ? NotFound() : Ok(progress);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

}
