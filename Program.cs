using Mentorly.Components;
using Mentorly.Application.Abstractions.Identity;
using Mentorly.Application;
using Mentorly.Infrastructure;
using Mentorly.Infrastructure.Identity;
using Mentorly.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=mentorly.db";

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
var googleIsConfigured =
    !string.IsNullOrWhiteSpace(googleClientId) &&
    !string.IsNullOrWhiteSpace(googleClientSecret);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(defaultConnection);

builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole<Guid>>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddEntityFrameworkStores<MentorlyDbContext>();

var authenticationBuilder = builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    });

authenticationBuilder.AddIdentityCookies();

if (googleIsConfigured)
{
    authenticationBuilder.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = googleClientId!;
        options.ClientSecret = googleClientSecret!;
        options.SignInScheme = IdentityConstants.ExternalScheme;
    });
}

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MentorlyDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/auth/login", async (HttpContext context) =>
{
    if (!googleIsConfigured)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync("Google OAuth is not configured. Set Authentication:Google:ClientId and ClientSecret.");
        return;
    }

    var returnUrl = context.Request.Query["returnUrl"].ToString();
    if (string.IsNullOrWhiteSpace(returnUrl))
    {
        returnUrl = "/";
    }

    var redirectUrl = $"/auth/callback?returnUrl={Uri.EscapeDataString(returnUrl)}";
    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

    await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
});

app.MapGet("/auth/callback", async (
    HttpContext context,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IStudentIdentityMapper studentIdentityMapper) =>
{
    var returnUrl = context.Request.Query["returnUrl"].ToString();
    if (string.IsNullOrWhiteSpace(returnUrl))
    {
        returnUrl = "/";
    }

    var externalAuth = await context.AuthenticateAsync(IdentityConstants.ExternalScheme);
    if (!externalAuth.Succeeded || externalAuth.Principal is null)
    {
        return Results.Redirect("/");
    }

    var principal = externalAuth.Principal;
    var googleId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
    var email = principal.FindFirstValue(ClaimTypes.Email);
    var displayName = principal.FindFirstValue(ClaimTypes.Name) ?? email;

    if (string.IsNullOrWhiteSpace(googleId) || string.IsNullOrWhiteSpace(email))
    {
        await context.SignOutAsync(IdentityConstants.ExternalScheme);
        return Results.BadRequest("Google claims are missing required values.");
    }

    var user = await userManager.FindByLoginAsync(GoogleDefaults.AuthenticationScheme, googleId);
    if (user is null)
    {
        user = await userManager.FindByEmailAsync(email);
    }

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
            await context.SignOutAsync(IdentityConstants.ExternalScheme);
            return Results.Problem("Unable to create user account.");
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
            await context.SignOutAsync(IdentityConstants.ExternalScheme);
            return Results.Problem("Unable to link Google login.");
        }
    }

    var studentId = await studentIdentityMapper.EnsureStudentAsync(principal, context.RequestAborted);

    var existingClaims = await userManager.GetClaimsAsync(user);
    var studentClaim = existingClaims.FirstOrDefault(x => x.Type == MentorlyClaimTypes.StudentId);
    if (studentClaim is null)
    {
        await userManager.AddClaimAsync(user, new Claim(MentorlyClaimTypes.StudentId, studentId.ToString()));
    }
    else if (studentClaim.Value != studentId.ToString())
    {
        await userManager.ReplaceClaimAsync(user, studentClaim, new Claim(MentorlyClaimTypes.StudentId, studentId.ToString()));
    }

    await context.SignOutAsync(IdentityConstants.ExternalScheme);
    await signInManager.SignInAsync(user, isPersistent: true);

    return Results.Redirect(returnUrl);
});

app.MapGet("/auth/logout", async (HttpContext context, SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();

    var returnUrl = context.Request.Query["returnUrl"].ToString();
    if (string.IsNullOrWhiteSpace(returnUrl))
    {
        returnUrl = "/";
    }

    return Results.Redirect(returnUrl);
});

app.Run();
