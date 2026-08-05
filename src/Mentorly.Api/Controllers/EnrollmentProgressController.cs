using System.Security.Claims;
using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Mentorly.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Authorize(Policy = MentorlyPolicies.Student)]
[Route("api")]
public class EnrollmentProgressController(IEnrollmentProgressService enrollmentProgressService) : ControllerBase
{
    [HttpGet("students/me/enrollments")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetMyEnrollmentsAsync(CancellationToken cancellationToken = default)
    {
        var studentId = GetStudentId();
        return studentId is null ? Unauthorized() : Ok(await enrollmentProgressService.GetStudentEnrollmentsAsync(studentId.Value, cancellationToken));
    }

    [HttpPost("courses/{courseId:guid}/enrollments/restart")]
    public async Task<ActionResult<EnrollmentDto>> RestartAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var studentId = GetStudentId();
        if (studentId is null) return Unauthorized();
        try
        {
            var enrollment = await enrollmentProgressService.RestartAsync(studentId.Value, courseId, cancellationToken);
            return enrollment is null ? NotFound() : CreatedAtAction(nameof(GetStatusAsync), new { enrollmentId = enrollment.Id }, enrollment);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpGet("enrollments/{enrollmentId:guid}/progress")]
    public async Task<ActionResult<EnrollmentProgressDto>> GetProgressAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var studentId = GetStudentId();
        if (studentId is null) return Unauthorized();
        var progress = await enrollmentProgressService.GetProgressAsync(enrollmentId, studentId.Value, cancellationToken);
        return progress is null ? NotFound() : Ok(progress);
    }

    [HttpPost("enrollments/{enrollmentId:guid}/themes/{themeId:guid}/complete")]
    public async Task<ActionResult<EnrollmentProgressDto>> CompleteThemeAsync(Guid enrollmentId, Guid themeId, CancellationToken cancellationToken = default)
    {
        var studentId = GetStudentId();
        if (studentId is null) return Unauthorized();
        try
        {
            var progress = await enrollmentProgressService.CompleteThemeAsync(enrollmentId, studentId.Value, themeId, cancellationToken);
            return progress is null ? NotFound() : Ok(progress);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpGet("enrollments/{enrollmentId:guid}/status")]
    public async Task<ActionResult<EnrollmentStatusDto>> GetStatusAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var studentId = GetStudentId();
        if (studentId is null) return Unauthorized();
        var status = await enrollmentProgressService.GetStatusAsync(enrollmentId, studentId.Value, cancellationToken);
        return status is null ? NotFound() : Ok(status);
    }

    [HttpGet("enrollments/{enrollmentId:guid}/certificate")]
    public async Task<ActionResult<CertificateDto>> GetCertificateAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var studentId = GetStudentId();
        if (studentId is null) return Unauthorized();
        var certificate = await enrollmentProgressService.GetCertificateAsync(enrollmentId, studentId.Value, cancellationToken);
        return certificate is null ? NotFound() : Ok(certificate);
    }

    private Guid? GetStudentId()
    {
        return Guid.TryParse(User.FindFirstValue(MentorlyClaimTypes.StudentId), out var studentId) ? studentId : null;
    }
}
