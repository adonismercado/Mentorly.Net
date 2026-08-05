using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Mentorly.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api")]
public class ActivitiesController(IActivityService activityService) : ControllerBase
{
    [Authorize(Policy = MentorlyPolicies.Student)]
    [HttpGet("themes/{themeId:guid}/activities")]
    public async Task<ActionResult<IEnumerable<ActivityDto>>> GetByThemeAsync(Guid themeId, CancellationToken cancellationToken = default) => Ok(await activityService.GetByThemeAsync(themeId, cancellationToken));

    [Authorize(Policy = MentorlyPolicies.Student)]
    [HttpGet("activities/{activityId:guid}")]
    public async Task<ActionResult<ActivityDto>> GetAsync(Guid activityId, CancellationToken cancellationToken = default)
    { var activity = await activityService.GetByIdAsync(activityId, cancellationToken); return activity is null ? NotFound() : Ok(activity); }

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpPost("themes/{themeId:guid}/activities")]
    public async Task<ActionResult<ActivityDto>> CreateAsync(Guid themeId, CreateActivityDto dto, CancellationToken cancellationToken = default)
    { var activity = await activityService.CreateAsync(themeId, dto, cancellationToken); return activity is null ? NotFound() : CreatedAtAction(nameof(GetByThemeAsync), new { themeId }, activity); }

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpPut("activities/{activityId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid activityId, UpdateActivityDto dto, CancellationToken cancellationToken = default) => await activityService.UpdateAsync(activityId, dto, cancellationToken) ? NoContent() : NotFound();

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpDelete("activities/{activityId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid activityId, CancellationToken cancellationToken = default)
    { try { return await activityService.DeleteAsync(activityId, cancellationToken) ? NoContent() : NotFound(); } catch (InvalidOperationException exception) { return Conflict(new { message = exception.Message }); } }

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpPatch("themes/{themeId:guid}/activities/reorder")]
    public async Task<IActionResult> ReorderAsync(Guid themeId, ReorderItemsDto dto, CancellationToken cancellationToken = default)
    { try { return await activityService.ReorderAsync(themeId, dto, cancellationToken) ? NoContent() : NotFound(); } catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); } }
}
