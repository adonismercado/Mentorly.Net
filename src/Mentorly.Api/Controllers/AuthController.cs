using System.Security.Claims;
using Mentorly.Application.Abstractions.Identity;
using Mentorly.Application.Services;
using Mentorly.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IConfiguration configuration,
    IStudentIdentityMapper studentIdentityMapper,
    IStudentService studentService,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("google")]
    [HttpPost("google")]
    public IActionResult LoginWithGoogle()
    {
        var clientId = configuration["Authentication:Google:ClientId"];
        var clientSecret = configuration["Authentication:Google:ClientSecret"];
        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            return Problem("Google OAuth is not configured.", statusCode: StatusCodes.Status500InternalServerError);
        }

        var redirectUrl = Url.ActionLink(nameof(GoogleCallback), "Auth")
            ?? throw new InvalidOperationException("Unable to generate the Google callback URL.");

        return Challenge(
            new AuthenticationProperties { RedirectUri = redirectUrl },
            GoogleDefaults.AuthenticationScheme);
    }

    [AllowAnonymous]
    [HttpGet("google/callback")]
    public async Task<ActionResult> GoogleCallback(CancellationToken cancellationToken = default)
    {
        var externalAuth = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (!externalAuth.Succeeded || externalAuth.Principal is null)
        {
            return Unauthorized();
        }

        var principal = externalAuth.Principal;
        var googleId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = principal.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(googleId) || string.IsNullOrWhiteSpace(email))
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return BadRequest("Google claims are missing required values.");
        }

        var user = await userManager.FindByLoginAsync(GoogleDefaults.AuthenticationScheme, googleId)
            ?? await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                return Problem("Unable to create user account.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        var userLogins = await userManager.GetLoginsAsync(user);
        if (!userLogins.Any(x => x.LoginProvider == GoogleDefaults.AuthenticationScheme && x.ProviderKey == googleId))
        {
            var addLoginResult = await userManager.AddLoginAsync(user, new UserLoginInfo(
                GoogleDefaults.AuthenticationScheme,
                googleId,
                GoogleDefaults.AuthenticationScheme));

            if (!addLoginResult.Succeeded)
            {
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                return Problem("Unable to link Google login.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        var studentId = await studentIdentityMapper.EnsureStudentAsync(principal, cancellationToken);
        user.StudentId = studentId;

        var studentClaim = (await userManager.GetClaimsAsync(user))
            .FirstOrDefault(x => x.Type == MentorlyClaimTypes.StudentId);
        var claim = new Claim(MentorlyClaimTypes.StudentId, studentId.ToString());
        var claimResult = studentClaim is null
            ? await userManager.AddClaimAsync(user, claim)
            : await userManager.ReplaceClaimAsync(user, studentClaim, claim);

        if (!claimResult.Succeeded)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return Problem("Unable to associate the user with the student profile.", statusCode: StatusCodes.Status500InternalServerError);
        }

        if (!await userManager.IsInRoleAsync(user, MentorlyRoles.Admin) &&
            !await userManager.IsInRoleAsync(user, MentorlyRoles.Student))
        {
            var roleResult = await userManager.AddToRoleAsync(user, MentorlyRoles.Student);
            if (!roleResult.Succeeded)
            {
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                return Problem("Unable to assign the student role.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return Problem("Unable to save the user profile.", statusCode: StatusCodes.Status500InternalServerError);
        }

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        await signInManager.SignInAsync(user, isPersistent: true);

        var student = await studentService.GetStudentByIdAsync(studentId, cancellationToken);
        return Ok(student);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var studentId = GetCurrentStudentId();
        if (studentId is null)
        {
            return Unauthorized();
        }

        var student = await studentService.GetStudentByIdAsync(studentId.Value, cancellationToken);
        return student is null ? NotFound() : Ok(student);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await signInManager.SignOutAsync();
        return NoContent();
    }

    private Guid? GetCurrentStudentId()
    {
        var value = User.FindFirstValue(MentorlyClaimTypes.StudentId);
        return Guid.TryParse(value, out var studentId) ? studentId : null;
    }
}
