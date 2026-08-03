using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionsController(ISubmissionService submissionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubmissionDto>>> GetSubmissionsAsync(CancellationToken cancellationToken = default)
    {
        var submissions = await submissionService.GetAllSubmissionsAsync(cancellationToken);
        return Ok(submissions);
    }

    [HttpGet("{id}")]
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