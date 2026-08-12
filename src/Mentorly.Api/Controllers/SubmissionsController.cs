using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api")]
public class SubmissionsController(ISubmissionService submissionService) : ControllerBase
{
    [HttpGet("admins/{adminId:guid}/submissions/pending-decision")]
    public async Task<ActionResult<SubmissionDto[]>> GetEscalatedSubmissionsAsync(
        Guid adminId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await submissionService.GetEscalatedSubmissionsAsync(adminId, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpGet("students/{studentId:guid}/submissions")]
    public async Task<ActionResult<IEnumerable<SubmissionDto>>> GetMySubmissionsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return Ok(await submissionService.GetMySubmissionsAsync(studentId, cancellationToken));
    }

    [HttpGet("students/{studentId:guid}/submissions/{id:guid}/reviews")]
    public async Task<ActionResult<IEnumerable<PeerReviewFeedbackDto>>> GetMySubmissionReviewsAsync(Guid studentId, Guid id, CancellationToken cancellationToken = default)
    {
        var reviews = await submissionService.GetMySubmissionReviewsAsync(id, studentId, cancellationToken);
        return reviews is null ? NotFound() : Ok(reviews);
    }

    [HttpPost("students/{studentId:guid}/submissions/{id:guid}/escalate")]
    public async Task<IActionResult> EscalateAsync(Guid studentId, Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await submissionService.EscalateAsync(id, studentId, cancellationToken) ? NoContent() : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPost("admins/{adminId:guid}/submissions/{id:guid}/decision")]
    public async Task<IActionResult> DecideAsync(Guid adminId, Guid id, AdminSubmissionDecisionDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            return await submissionService.DecideAsAdminAsync(adminId, id, dto.IsApproved, cancellationToken) ? NoContent() : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpGet("submissions/{id:guid}", Name = "GetSubmission")]
    public async Task<ActionResult<SubmissionDto>> GetSubmissionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var submission = await submissionService.GetSubmissionByIdAsync(id, cancellationToken);

        if (submission is null)
        {
            return NotFound();
        }

        return Ok(submission);
    }

    [HttpPost("enrollments/{enrollmentId:guid}/activities/{activityId:guid}/submissions")]
    public async Task<ActionResult<SubmissionDto>> CreateSubmissionAsync(Guid enrollmentId, Guid activityId, CreateSubmissionDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var submission = await submissionService.CreateSubmissionAsync(enrollmentId, activityId, dto, cancellationToken);
            return CreatedAtRoute("GetSubmission", new { id = submission.Id }, submission);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("submissions/{id:guid}")]
    public async Task<IActionResult> UpdateSubmissionAsync(Guid id, UpdateSubmissionDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var updated = await submissionService.UpdateSubmissionAsync(id, dto, cancellationToken);
            return updated ? NoContent() : NotFound();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
