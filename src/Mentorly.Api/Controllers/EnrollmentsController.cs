using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Mentorly.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController(
    IEnrollmentService enrollmentService,
    IStudentEnrollmentService studentEnrollmentService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = MentorlyPolicies.Admin)]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsAsync(CancellationToken cancellationToken = default)
    {
        var enrollments = await enrollmentService.GetAllEnrollmentsAsync(cancellationToken);
        return Ok(enrollments);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = MentorlyPolicies.Admin)]
    public async Task<ActionResult<EnrollmentDto>> GetEnrollmentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentService.GetEnrollmentByIdAsync(id, cancellationToken);

        if (enrollment is null)
        {
            return NotFound();
        }

        return Ok(enrollment);
    }

    [HttpPost]
    [Authorize(Policy = MentorlyPolicies.Student)]
    public async Task<ActionResult<EnrollmentResultDto>> CreateEnrollmentAsync(CreateEnrollmentDto dto, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(MentorlyClaimTypes.StudentId), out var studentId))
        {
            return Unauthorized();
        }

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
