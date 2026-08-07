using Microsoft.AspNetCore.Identity;

namespace Mentorly.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public Guid? StudentId { get; set; }
}
