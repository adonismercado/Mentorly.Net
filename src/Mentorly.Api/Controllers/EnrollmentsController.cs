using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController(IEnrollmentService enrollmentService) : ControllerBase
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

    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> CreateEnrollmentAsync(CreateEnrollmentDto dto, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentService.CreateEnrollmentAsync(dto, cancellationToken);
        return CreatedAtAction("GetEnrollment", new { id = enrollment.Id }, enrollment);
    }
}
