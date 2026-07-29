using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Tests.Domain;

public sealed class EnrollmentTests
{
    [Fact]
    public void CreateNew_SetsExpirationAtExactlyThreeMonths()
    {
        var startedAt = new DateTime(2026, 01, 10, 14, 30, 00, DateTimeKind.Utc);

        var enrollment = Enrollment.CreateNew(Guid.NewGuid(), Guid.NewGuid(), 1, startedAt);

        Assert.Equal(startedAt.AddMonths(3), enrollment.ExpiresAt);
        Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
    }

    [Fact]
    public void RefreshStatus_SetsExpired_WhenNowIsAfterExpiration()
    {
        var startedAt = new DateTime(2026, 01, 01, 00, 00, 00, DateTimeKind.Utc);
        var enrollment = Enrollment.CreateNew(Guid.NewGuid(), Guid.NewGuid(), 1, startedAt);

        enrollment.RefreshStatus(enrollment.ExpiresAt.AddSeconds(1));

        Assert.Equal(EnrollmentStatus.Expired, enrollment.Status);
        Assert.False(enrollment.CanSubmit(enrollment.ExpiresAt.AddSeconds(1)));
    }
}
