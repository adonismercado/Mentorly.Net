using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api")]
public class PeerReviewsController(IPeerReviewService peerReviewService) : ControllerBase
{
    [HttpGet("activities/{activityId:guid}/peer-review-rubric")]
    public async Task<ActionResult<PeerReviewRubricCriterionDto[]>> GetRubricAsync(Guid activityId, CancellationToken cancellationToken = default) => Ok(await peerReviewService.GetRubricAsync(activityId, cancellationToken));

    [HttpPost("admins/{adminId:guid}/activities/{activityId:guid}/peer-review-rubric/criteria")]
    public async Task<ActionResult<PeerReviewRubricCriterionDto>> CreateRubricCriterionAsync(Guid adminId, Guid activityId, CreatePeerReviewRubricCriterionDto dto, CancellationToken cancellationToken = default)
    {
        try { return Created($"/api/activities/{activityId}/peer-review-rubric", await peerReviewService.CreateRubricCriterionAsync(adminId, activityId, dto, cancellationToken)); }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
        catch (InvalidOperationException exception) { return Conflict(new { message = exception.Message }); }
    }

    [HttpPut("admins/{adminId:guid}/peer-review-rubric/criteria/{criterionId:guid}")]
    public async Task<IActionResult> UpdateRubricCriterionAsync(Guid adminId, Guid criterionId, UpdatePeerReviewRubricCriterionDto dto, CancellationToken cancellationToken = default)
    {
        try { return await peerReviewService.UpdateRubricCriterionAsync(adminId, criterionId, dto, cancellationToken) ? NoContent() : NotFound(); }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
        catch (InvalidOperationException exception) { return Conflict(new { message = exception.Message }); }
    }

    [HttpDelete("admins/{adminId:guid}/peer-review-rubric/criteria/{criterionId:guid}")]
    public async Task<IActionResult> DeleteRubricCriterionAsync(Guid adminId, Guid criterionId, CancellationToken cancellationToken = default)
    {
        try { return await peerReviewService.DeleteRubricCriterionAsync(adminId, criterionId, cancellationToken) ? NoContent() : NotFound(); }
        catch (InvalidOperationException exception) { return Conflict(new { message = exception.Message }); }
    }
    [HttpGet("students/{studentId:guid}/peer-review-queue")]
    public async Task<ActionResult<IEnumerable<ReviewQueueItemDto>>> GetQueueAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await peerReviewService.GetEligibleQueueAsync(studentId, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

    [HttpGet("students/{studentId:guid}/peer-reviews")]
    public async Task<ActionResult<IEnumerable<PeerReviewDto>>> GetMyReviewsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return Ok(await peerReviewService.GetMyPeerReviewsAsync(studentId, cancellationToken));
    }

    [HttpGet("students/{studentId:guid}/peer-review-queue/{submissionId:guid}")]
    public async Task<ActionResult<AnonymousSubmissionDto>> GetAnonymousSubmissionAsync(Guid studentId, Guid submissionId, CancellationToken cancellationToken = default)
    {
        var submission = await peerReviewService.GetAnonymousSubmissionAsync(submissionId, studentId, cancellationToken);
        return submission is null ? NotFound() : Ok(submission);
    }

    [HttpGet("admins/{adminId:guid}/peer-reviews/{peerReviewId:guid}/audit")]
    public async Task<ActionResult<PeerReviewAuditDto>> GetAuditAsync(Guid adminId, Guid peerReviewId, CancellationToken cancellationToken = default)
    {
        try
        {
            var audit = await peerReviewService.GetAuditAsync(adminId, peerReviewId, cancellationToken);
            return audit is null ? NotFound() : Ok(audit);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpGet("admins/{adminId:guid}/peer-reviews")]
    public async Task<ActionResult<IEnumerable<PeerReviewDto>>> GetPeerReviewsAsync(Guid adminId, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await peerReviewService.GetAllPeerReviewsAsync(adminId, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPost("students/{studentId:guid}/peer-reviews")]
    public async Task<ActionResult<PeerReviewResultDto>> SubmitReviewAsync(Guid studentId, CreatePeerReviewRequestDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await peerReviewService.SubmitReviewAsync(studentId, dto, cancellationToken);
            return Created($"/api/students/{studentId}/peer-reviews", result);
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
