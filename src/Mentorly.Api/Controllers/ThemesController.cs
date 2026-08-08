using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api")]
public class ThemesController(IThemeService themeService) : ControllerBase
{
    [HttpGet("units/{unitId:guid}/themes")]
    public async Task<ActionResult<IEnumerable<ThemeDto>>> GetAsync(Guid unitId, CancellationToken cancellationToken = default) => Ok(await themeService.GetByUnitAsync(unitId, cancellationToken));

    [HttpPost("units/{unitId:guid}/themes")]
    public async Task<ActionResult<ThemeDto>> CreateAsync(Guid unitId, CreateThemeDto dto, CancellationToken cancellationToken = default)
    { var theme = await themeService.CreateAsync(unitId, dto, cancellationToken); return theme is null ? NotFound() : CreatedAtAction(nameof(GetAsync), new { unitId }, theme); }

    [HttpPut("themes/{themeId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid themeId, UpdateThemeDto dto, CancellationToken cancellationToken = default) => await themeService.UpdateAsync(themeId, dto, cancellationToken) ? NoContent() : NotFound();

    [HttpDelete("themes/{themeId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid themeId, CancellationToken cancellationToken = default)
    { try { return await themeService.DeleteAsync(themeId, cancellationToken) ? NoContent() : NotFound(); } catch (InvalidOperationException exception) { return Conflict(new { message = exception.Message }); } }

    [HttpPatch("units/{unitId:guid}/themes/reorder")]
    public async Task<IActionResult> ReorderAsync(Guid unitId, ReorderItemsDto dto, CancellationToken cancellationToken = default)
    { try { return await themeService.ReorderAsync(unitId, dto, cancellationToken) ? NoContent() : NotFound(); } catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); } }
}
