using System.Security.Claims;

namespace Mentorly.Application.Abstractions.Identity;

public interface IStudentIdentityMapper
{
    Task<Guid> EnsureStudentAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
}
