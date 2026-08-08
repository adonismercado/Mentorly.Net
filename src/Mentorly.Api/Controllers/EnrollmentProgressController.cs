using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api")]
public class EnrollmentProgressController(IEnrollmentProgressService enrollmentProgressService) : ControllerBase
{
    [HttpGet("students/{studentId:guid}/enrollments")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetStudentEnrollmentsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return Ok(await enrollmentProgressService.GetStudentEnrollmentsAsync(studentId, cancellationToken));
    }

    [HttpPost("students/{studentId:guid}/courses/{courseId:guid}/enrollments/restart")]
    public async Task<ActionResult<EnrollmentDto>> RestartAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            var enrollment = await enrollmentProgressService.RestartAsync(studentId, courseId, cancellationToken);
            return enrollment is null ? NotFound() : CreatedAtAction(nameof(GetStatusAsync), new { enrollmentId = enrollment.Id }, enrollment);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpGet("enrollments/{enrollmentId:guid}/progress")]
    public async Task<ActionResult<EnrollmentProgressDto>> GetProgressAsync(Guid enrollmentId, [FromQuery] Guid studentId, CancellationToken cancellationToken = default)
    {
        var progress = await enrollmentProgressService.GetProgressAsync(enrollmentId, studentId, cancellationToken);
        return progress is null ? NotFound() : Ok(progress);
    }

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

    [HttpGet("enrollments/{enrollmentId:guid}/status")]
    public async Task<ActionResult<EnrollmentStatusDto>> GetStatusAsync(Guid enrollmentId, [FromQuery] Guid studentId, CancellationToken cancellationToken = default)
    {
        var status = await enrollmentProgressService.GetStatusAsync(enrollmentId, studentId, cancellationToken);
        return status is null ? NotFound() : Ok(status);
    }

    [HttpGet("enrollments/{enrollmentId:guid}/certificate")]
    public async Task<ActionResult<CertificateDto>> GetCertificateAsync(Guid enrollmentId, [FromQuery] Guid studentId, CancellationToken cancellationToken = default)
    {
        var certificate = await enrollmentProgressService.GetCertificateAsync(enrollmentId, studentId, cancellationToken);
        return certificate is null ? NotFound() : Ok(certificate);
    }
}
