using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Mentorly.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionsController(ISubmissionService submissionService) : ControllerBase
{
    [Authorize(Policy = MentorlyPolicies.Student)]
    [HttpGet("me")]
    public async Task<ActionResult<IEnumerable<SubmissionDto>>> GetMySubmissionsAsync(CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(MentorlyClaimTypes.StudentId), out var studentId)) return Unauthorized();
        return Ok(await submissionService.GetMySubmissionsAsync(studentId, cancellationToken));
    }

    [Authorize(Policy = MentorlyPolicies.Student)]
    [HttpGet("{id}/reviews")]
    public async Task<ActionResult<IEnumerable<PeerReviewFeedbackDto>>> GetMySubmissionReviewsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(MentorlyClaimTypes.StudentId), out var studentId)) return Unauthorized();
        var reviews = await submissionService.GetMySubmissionReviewsAsync(id, studentId, cancellationToken);
        return reviews is null ? NotFound() : Ok(reviews);
    }

    [Authorize(Policy = MentorlyPolicies.Student)]
    [HttpPost("{id}/escalate")]
    public async Task<IActionResult> EscalateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(MentorlyClaimTypes.StudentId), out var studentId)) return Unauthorized();
        return await submissionService.EscalateAsync(id, studentId, cancellationToken) ? NoContent() : NotFound();
    }

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpPost("{id}/admin-decision")]
    public async Task<IActionResult> DecideAsync(Guid id, AdminSubmissionDecisionDto dto, CancellationToken cancellationToken = default)
        => await submissionService.DecideAsAdminAsync(id, dto.IsApproved, cancellationToken) ? NoContent() : NotFound();

    [HttpGet]
    [Authorize(Policy = MentorlyPolicies.Admin)]
    public async Task<ActionResult<IEnumerable<SubmissionDto>>> GetSubmissionsAsync(CancellationToken cancellationToken = default)
    {
        var submissions = await submissionService.GetAllSubmissionsAsync(cancellationToken);
        return Ok(submissions);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = MentorlyPolicies.Admin)]
    public async Task<ActionResult<SubmissionDto>> GetSubmissionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var submission = await submissionService.GetSubmissionByIdAsync(id, cancellationToken);

        if (submission is null)
        {
            return NotFound();
        }

        return Ok(submission);
    }

    [HttpPost]
    public async Task<ActionResult<SubmissionDto>> CreateSubmissionAsync(CreateSubmissionDto dto, CancellationToken cancellationToken = default)
    {
        var submission = await submissionService.CreateSubmissionAsync(dto, cancellationToken);
        return CreatedAtAction("GetSubmission", new { id = submission.Id }, submission);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubmissionAsync(Guid id, UpdateSubmissionDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await submissionService.UpdateSubmissionAsync(id, dto, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubmissionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await submissionService.DeleteSubmissionAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
