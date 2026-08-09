using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api")]
public class ActivitiesController(IActivityService activityService) : ControllerBase
{
    [HttpGet("themes/{themeId:guid}/activities", Name = "GetThemeActivities")]
    public async Task<ActionResult<IEnumerable<ActivityDto>>> GetAsync(Guid themeId, CancellationToken cancellationToken = default)
        => Ok(await activityService.GetByThemeAsync(themeId, cancellationToken));

    [HttpPost("admins/{adminId:guid}/themes/{themeId:guid}/activities")]
    public async Task<ActionResult<ActivityDto>> CreateAsync(Guid adminId, Guid themeId, CreateActivityDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = await activityService.CreateAsync(adminId, themeId, dto, cancellationToken);
            return activity is null ? NotFound() : CreatedAtRoute("GetThemeActivities", new { themeId }, activity);
        }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
    }

    [HttpPut("admins/{adminId:guid}/activities/{activityId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid adminId, Guid activityId, UpdateActivityDto dto, CancellationToken cancellationToken = default)
    {
        try { return await activityService.UpdateAsync(adminId, activityId, dto, cancellationToken) ? NoContent() : NotFound(); }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
    }

    [HttpDelete("admins/{adminId:guid}/activities/{activityId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid adminId, Guid activityId, CancellationToken cancellationToken = default)
    {
        try { return await activityService.DeleteAsync(adminId, activityId, cancellationToken) ? NoContent() : NotFound(); }
        catch (InvalidOperationException exception) { return Conflict(new { message = exception.Message }); }
    }

    [HttpPatch("admins/{adminId:guid}/themes/{themeId:guid}/activities/order")]
    public async Task<IActionResult> ReorderAsync(Guid adminId, Guid themeId, ReorderItemsDto dto, CancellationToken cancellationToken = default)
    {
        try { return await activityService.ReorderAsync(adminId, themeId, dto, cancellationToken) ? NoContent() : NotFound(); }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
    }
}
