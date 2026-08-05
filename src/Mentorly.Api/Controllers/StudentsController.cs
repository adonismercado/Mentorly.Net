using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Mentorly.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController(
    IStudentService studentService,
    UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudentsAsync(CancellationToken cancellationToken = default)
    {
        var students = await studentService.GetAllStudentsAsync(cancellationToken);
        return Ok(students);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StudentDto>> GetStudentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await studentService.GetStudentByIdAsync(id, cancellationToken);

        if (student is null)
        {
            return NotFound();
        }

        return Ok(student);
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> CreateStudentAsync(CreateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = await studentService.CreateStudentAsync(dto, cancellationToken);
        return CreatedAtAction("GetStudent", new { id = student.Id }, student);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudentAsync(Guid id, UpdateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await studentService.UpdateStudentAsync(id, dto, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await studentService.DeleteStudentAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Policy = MentorlyPolicies.Student)]
    [HttpPatch("me/privacy")]
    public async Task<IActionResult> UpdateMyPrivacyAsync(UpdateLeaderboardPrivacyDto dto, CancellationToken cancellationToken = default)
    {
        var studentId = GetCurrentStudentId();
        if (studentId is null)
        {
            return Unauthorized();
        }

        var updated = await studentService.UpdateLeaderboardPrivacyAsync(studentId.Value, dto.IsLeaderboardPublic, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [Authorize(Policy = MentorlyPolicies.Student)]
    [HttpGet("me/statistics")]
    public async Task<ActionResult<StudentStatisticsDto>> GetMyStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var studentId = GetCurrentStudentId();
        if (studentId is null)
        {
            return Unauthorized();
        }

        var statistics = await studentService.GetStudentStatisticsAsync(studentId.Value, cancellationToken);
        return statistics is null ? NotFound() : Ok(statistics);
    }

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpPost("{studentId}/promote-admin")]
    public async Task<IActionResult> PromoteToAdminAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var promoted = await studentService.PromoteToAdminAsync(studentId, cancellationToken);
        if (!promoted)
        {
            return NotFound();
        }

        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.StudentId == studentId, cancellationToken);

        if (user is not null && !await userManager.IsInRoleAsync(user, MentorlyRoles.Admin))
        {
            var addRoleResult = await userManager.AddToRoleAsync(user, MentorlyRoles.Admin);
            if (!addRoleResult.Succeeded)
            {
                return Problem("Unable to assign the administrator role.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        return NoContent();
    }

    private Guid? GetCurrentStudentId()
    {
        var value = User.FindFirstValue(MentorlyClaimTypes.StudentId);
        return Guid.TryParse(value, out var studentId) ? studentId : null;
    }
}
