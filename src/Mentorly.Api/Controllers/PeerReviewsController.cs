using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Mentorly.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeerReviewsController(IPeerReviewService peerReviewService) : ControllerBase
{
    [Authorize(Policy = MentorlyPolicies.Student)]
    [HttpGet("queue")]
    public async Task<ActionResult<IEnumerable<ReviewQueueItemDto>>> GetQueueAsync(CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(MentorlyClaimTypes.StudentId), out var studentId))
        {
            return Unauthorized();
        }

        try
        {
            return Ok(await peerReviewService.GetEligibleQueueAsync(studentId, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

    [Authorize(Policy = MentorlyPolicies.Student)]
    [HttpGet("me")]
    public async Task<ActionResult<IEnumerable<PeerReviewDto>>> GetMyReviewsAsync(CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(MentorlyClaimTypes.StudentId), out var studentId)) return Unauthorized();
        return Ok(await peerReviewService.GetMyPeerReviewsAsync(studentId, cancellationToken));
    }

    [Authorize(Policy = MentorlyPolicies.Student)]
    [HttpGet("{id:guid}/anonymous-submission")]
    public async Task<ActionResult<AnonymousSubmissionDto>> GetAnonymousSubmissionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(MentorlyClaimTypes.StudentId), out var studentId)) return Unauthorized();
        var submission = await peerReviewService.GetAnonymousSubmissionAsync(id, studentId, cancellationToken);
        return submission is null ? NotFound() : Ok(submission);
    }

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpGet("/api/admin/peer-reviews/{id:guid}/audit")]
    public async Task<ActionResult<PeerReviewAuditDto>> GetAuditAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var audit = await peerReviewService.GetAuditAsync(id, cancellationToken);
        return audit is null ? NotFound() : Ok(audit);
    }

    [HttpGet]
    [Authorize(Policy = MentorlyPolicies.Admin)]
    public async Task<ActionResult<IEnumerable<PeerReviewDto>>> GetPeerReviewsAsync(CancellationToken cancellationToken = default)
    {
        var peerReviews = await peerReviewService.GetAllPeerReviewsAsync(cancellationToken);
        return Ok(peerReviews);
    }

    [HttpGet("{id:guid}", Name = "GetPeerReview")]
    public async Task<ActionResult<PeerReviewDto>> GetPeerReviewAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var peerReview = await peerReviewService.GetPeerReviewByIdAsync(id, cancellationToken);

        if (peerReview is null)
        {
            return NotFound();
        }

        return Ok(peerReview);
    }

    [HttpPost]
    [Authorize(Policy = MentorlyPolicies.Student)]
    public async Task<ActionResult<PeerReviewResultDto>> SubmitReviewAsync(CreatePeerReviewRequestDto dto, CancellationToken cancellationToken = default)
    {
        var result = await peerReviewService.SubmitReviewAsync(dto, cancellationToken);
        return CreatedAtRoute("GetPeerReview", new { id = result.PeerReviewId }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePeerReviewAsync(Guid id, UpdatePeerReviewDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await peerReviewService.UpdatePeerReviewAsync(id, dto, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePeerReviewAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await peerReviewService.DeletePeerReviewAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
