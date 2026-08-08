using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api")]
public class UnitsController(IUnitService unitService) : ControllerBase
{
    [HttpGet("courses/{courseId:guid}/units")]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetAsync(Guid courseId, CancellationToken cancellationToken = default)
        => Ok(await unitService.GetByCourseAsync(courseId, cancellationToken));

    [HttpPost("admins/{adminId:guid}/courses/{courseId:guid}/units")]
    public async Task<ActionResult<UnitDto>> CreateAsync(Guid adminId, Guid courseId, CreateUnitDto dto, CancellationToken cancellationToken = default)
    {
        var unit = await unitService.CreateAsync(adminId, courseId, dto, cancellationToken);
        return unit is null ? NotFound() : CreatedAtAction(nameof(GetAsync), new { courseId }, unit);
    }

    [HttpPut("admins/{adminId:guid}/units/{unitId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid adminId, Guid unitId, UpdateUnitDto dto, CancellationToken cancellationToken = default)
        => await unitService.UpdateAsync(adminId, unitId, dto, cancellationToken) ? NoContent() : NotFound();

    [HttpDelete("admins/{adminId:guid}/units/{unitId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid adminId, Guid unitId, CancellationToken cancellationToken = default)
    {
        try { return await unitService.DeleteAsync(adminId, unitId, cancellationToken) ? NoContent() : NotFound(); }
        catch (InvalidOperationException exception) { return Conflict(new { message = exception.Message }); }
    }

    [HttpPatch("admins/{adminId:guid}/courses/{courseId:guid}/units/order")]
    public async Task<IActionResult> ReorderAsync(Guid adminId, Guid courseId, ReorderItemsDto dto, CancellationToken cancellationToken = default)
    {
        try { return await unitService.ReorderAsync(adminId, courseId, dto, cancellationToken) ? NoContent() : NotFound(); }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
    }
}
