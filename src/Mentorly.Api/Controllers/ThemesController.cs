using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api")]
public class ThemesController(IThemeService themeService) : ControllerBase
{
    [HttpGet("units/{unitId:guid}/themes")]
    public async Task<ActionResult<IEnumerable<ThemeDto>>> GetAsync(Guid unitId, CancellationToken cancellationToken = default)
        => Ok(await themeService.GetByUnitAsync(unitId, cancellationToken));

    [HttpPost("admins/{adminId:guid}/units/{unitId:guid}/themes")]
    public async Task<ActionResult<ThemeDto>> CreateAsync(Guid adminId, Guid unitId, CreateThemeDto dto, CancellationToken cancellationToken = default)
    {
        var theme = await themeService.CreateAsync(adminId, unitId, dto, cancellationToken);
        return theme is null ? NotFound() : CreatedAtAction(nameof(GetAsync), new { unitId }, theme);
    }

    [HttpPut("admins/{adminId:guid}/themes/{themeId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid adminId, Guid themeId, UpdateThemeDto dto, CancellationToken cancellationToken = default)
        => await themeService.UpdateAsync(adminId, themeId, dto, cancellationToken) ? NoContent() : NotFound();

    [HttpDelete("admins/{adminId:guid}/themes/{themeId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid adminId, Guid themeId, CancellationToken cancellationToken = default)
    {
        try { return await themeService.DeleteAsync(adminId, themeId, cancellationToken) ? NoContent() : NotFound(); }
        catch (InvalidOperationException exception) { return Conflict(new { message = exception.Message }); }
    }

    [HttpPatch("admins/{adminId:guid}/units/{unitId:guid}/themes/order")]
    public async Task<IActionResult> ReorderAsync(Guid adminId, Guid unitId, ReorderItemsDto dto, CancellationToken cancellationToken = default)
    {
        try { return await themeService.ReorderAsync(adminId, unitId, dto, cancellationToken) ? NoContent() : NotFound(); }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
    }
}
