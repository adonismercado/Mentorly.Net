using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeerReviewsController(IPeerReviewService peerReviewService) : ControllerBase
{
    [HttpGet]
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