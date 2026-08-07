using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Mentorly.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/courses/{courseId:guid}/units")]
public class UnitsController(IUnitService unitService) : ControllerBase
{
    [Authorize(Policy = MentorlyPolicies.Student)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetAsync(Guid courseId, CancellationToken cancellationToken = default) => Ok(await unitService.GetByCourseAsync(courseId, cancellationToken));

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpPost]
    public async Task<ActionResult<UnitDto>> CreateAsync(Guid courseId, CreateUnitDto dto, CancellationToken cancellationToken = default)
    { var unit = await unitService.CreateAsync(courseId, dto, cancellationToken); return unit is null ? NotFound() : CreatedAtAction(nameof(GetAsync), new { courseId }, unit); }

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpPut("{unitId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid courseId, Guid unitId, UpdateUnitDto dto, CancellationToken cancellationToken = default) => await unitService.UpdateAsync(courseId, unitId, dto, cancellationToken) ? NoContent() : NotFound();

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpDelete("{unitId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid courseId, Guid unitId, CancellationToken cancellationToken = default)
    { try { return await unitService.DeleteAsync(courseId, unitId, cancellationToken) ? NoContent() : NotFound(); } catch (InvalidOperationException exception) { return Conflict(new { message = exception.Message }); } }

    [Authorize(Policy = MentorlyPolicies.Admin)]
    [HttpPatch("reorder")]
    public async Task<IActionResult> ReorderAsync(Guid courseId, ReorderItemsDto dto, CancellationToken cancellationToken = default)
    { try { return await unitService.ReorderAsync(courseId, dto, cancellationToken) ? NoContent() : NotFound(); } catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); } }
}
