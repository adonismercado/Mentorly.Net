using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController(
    IEnrollmentService enrollmentService,
    IStudentEnrollmentService studentEnrollmentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsAsync(CancellationToken cancellationToken = default)
    {
        var enrollments = await enrollmentService.GetAllEnrollmentsAsync(cancellationToken);
        return Ok(enrollments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EnrollmentDto>> GetEnrollmentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentService.GetEnrollmentByIdAsync(id, cancellationToken);

        if (enrollment is null)
        {
            return NotFound();
        }

        return Ok(enrollment);
    }

    [HttpPost("students/{studentId:guid}")]
    public async Task<ActionResult<EnrollmentResultDto>> CreateEnrollmentAsync(Guid studentId, CreateEnrollmentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var enrollment = await studentEnrollmentService.EnrollAsync(new CreateEnrollmentRequestDto(studentId, dto.CourseId, DateTime.UtcNow), cancellationToken);
            return CreatedAtAction("GetEnrollment", new { id = enrollment.EnrollmentId }, enrollment);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
