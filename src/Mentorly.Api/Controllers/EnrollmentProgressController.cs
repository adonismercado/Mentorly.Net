using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api")]
public class EnrollmentProgressController(IEnrollmentProgressService enrollmentProgressService) : ControllerBase
{
    [HttpPost("enrollments/{enrollmentId:guid}/themes/{themeId:guid}/complete")]
    public async Task<ActionResult<EnrollmentProgressDto>> CompleteThemeAsync(Guid enrollmentId, Guid themeId, [FromQuery] Guid studentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var progress = await enrollmentProgressService.CompleteThemeAsync(enrollmentId, studentId, themeId, cancellationToken);
            return progress is null ? NotFound() : Ok(progress);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

}
