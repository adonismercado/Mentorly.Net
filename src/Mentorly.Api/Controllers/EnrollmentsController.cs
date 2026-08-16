using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api")]
public class EnrollmentsController(
    IEnrollmentService enrollmentService,
    IStudentEnrollmentService studentEnrollmentService,
    IEnrollmentProgressService enrollmentProgressService) : ControllerBase
{
    [HttpPost("students/{studentId:guid}/enrollments")]
    public async Task<ActionResult<EnrollmentResultDto>> CreateEnrollmentAsync(Guid studentId, CreateEnrollmentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var enrollment = await studentEnrollmentService.EnrollAsync(
                new CreateEnrollmentRequestDto(studentId, dto.CourseId, DateTime.UtcNow),
                cancellationToken);

            return CreatedAtRoute("GetEnrollment", new { enrollmentId = enrollment.EnrollmentId }, enrollment);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpGet("students/{studentId:guid}/enrollments")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetStudentEnrollmentsAsync(Guid studentId, CancellationToken cancellationToken = default)
        => Ok(await enrollmentProgressService.GetStudentEnrollmentsAsync(studentId, cancellationToken));

    [HttpGet("admins/{adminId:guid}/students/{studentId:guid}/enrollments")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetStudentEnrollmentsAsAdminAsync(Guid adminId, Guid studentId, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await enrollmentProgressService.GetStudentEnrollmentsAsAdminAsync(adminId, studentId, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpGet("enrollments/{enrollmentId:guid}", Name = "GetEnrollment")]
    public async Task<ActionResult<EnrollmentDto>> GetEnrollmentAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentService.GetEnrollmentByIdAsync(enrollmentId, cancellationToken);
        return enrollment is null ? NotFound() : Ok(enrollment);
    }

    [HttpPost("students/{studentId:guid}/courses/{courseId:guid}/enrollments/restart")]
    public async Task<ActionResult<EnrollmentDto>> RestartAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            var enrollment = await enrollmentProgressService.RestartAsync(studentId, courseId, cancellationToken);
            return enrollment is null ? NotFound() : CreatedAtRoute("GetEnrollment", new { enrollmentId = enrollment.Id }, enrollment);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpGet("enrollments/{enrollmentId:guid}/status")]
    public async Task<ActionResult<EnrollmentStatusDto>> GetStatusAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var status = await enrollmentProgressService.GetStatusAsync(enrollmentId, cancellationToken);
        return status is null ? NotFound() : Ok(status);
    }

    [HttpGet("enrollments/{enrollmentId:guid}/certificate")]
    public async Task<ActionResult<CertificateDto>> GetCertificateAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var certificate = await enrollmentProgressService.GetCertificateAsync(enrollmentId, cancellationToken);
        return certificate is null ? NotFound() : Ok(certificate);
    }
}
