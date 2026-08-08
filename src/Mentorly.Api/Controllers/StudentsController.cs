using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController(IStudentService studentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudentsAsync(CancellationToken cancellationToken = default)
    {
        var students = await studentService.GetAllStudentsAsync(cancellationToken);
        return Ok(students);
    }

    [HttpGet("{studentId:guid}")]
    public async Task<ActionResult<StudentProfileDto>> GetStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await studentService.GetStudentByIdAsync(studentId, cancellationToken);

        if (student is null)
        {
            return NotFound();
        }

        return Ok(student);
    }

    [HttpPost("provision")]
    public async Task<ActionResult<StudentProfileDto>> ProvisionStudentAsync(ProvisionStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = await studentService.ProvisionStudentAsync(dto, cancellationToken);
        return Ok(student);
    }

    [HttpPut("{studentId:guid}")]
    public async Task<IActionResult> UpdateStudentAsync(Guid studentId, UpdateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await studentService.UpdateStudentAsync(studentId, dto, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPatch("{studentId:guid}/privacy")]
    public async Task<IActionResult> UpdatePrivacyAsync(Guid studentId, UpdateLeaderboardPrivacyDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await studentService.UpdateLeaderboardPrivacyAsync(studentId, dto.IsLeaderboardPublic, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpGet("{studentId:guid}/statistics")]
    public async Task<ActionResult<StudentStatisticsDto>> GetStatisticsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var statistics = await studentService.GetStudentStatisticsAsync(studentId, cancellationToken);
        return statistics is null ? NotFound() : Ok(statistics);
    }

    [HttpPost("/api/admins/{adminId:guid}/students/{studentId:guid}/promote")]
    public async Task<IActionResult> PromoteToAdminAsync(Guid adminId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var promoted = await studentService.PromoteToAdminAsync(adminId, studentId, cancellationToken);
        if (!promoted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
