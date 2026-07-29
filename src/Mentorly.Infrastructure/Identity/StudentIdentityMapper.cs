using System.Security.Claims;
using Mentorly.Application.Abstractions.Identity;
using Mentorly.Domain.Entities;
using Mentorly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Identity;

public sealed class StudentIdentityMapper(MentorlyDbContext dbContext) : IStudentIdentityMapper
{
    public async Task<Guid> EnsureStudentAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        if (principal.Identity?.IsAuthenticated is not true)
        {
            throw new InvalidOperationException("Unauthenticated principal cannot be mapped to student.");
        }

        var googleUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Google user identifier claim is missing.");

        var email = principal.FindFirstValue(ClaimTypes.Email)
            ?? throw new InvalidOperationException("Email claim is missing.");

        var displayName = principal.FindFirstValue(ClaimTypes.Name)
            ?? email;

        var student = await dbContext.Students
            .FirstOrDefaultAsync(x => x.GoogleUserId == googleUserId, cancellationToken);

        if (student is null)
        {
            student = new Student(Guid.NewGuid(), googleUserId, email, displayName);
            await dbContext.Students.AddAsync(student, cancellationToken);
        }
        else
        {
            student.UpdateProfile(email, displayName);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return student.Id;
    }
}
