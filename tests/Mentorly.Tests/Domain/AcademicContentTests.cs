using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Tests.Domain;

public sealed class AcademicContentTests
{
    [Fact]
    public void Hierarchy_Constructors_CreateValidContent()
    {
        var unit = new Unit(Guid.NewGuid(), Guid.NewGuid(), "Unit 1", 1);
        var theme = new Theme(Guid.NewGuid(), unit.Id, "Theme 1", "Content", 1);
        var activity = new Activity(Guid.NewGuid(), theme.Id, "Exercise", ActivityType.Exercise, true, ApprovalStrategy.PeerReview, 1);

        Assert.Equal(unit.Id, theme.UnitId);
        Assert.Equal(theme.Id, activity.ThemeId);
        Assert.Equal(ApprovalStrategy.PeerReview, activity.ApprovalStrategy);
    }

    [Fact]
    public void Activity_RejectsManualQuizApproval()
    {
        var action = () => new Activity(Guid.NewGuid(), Guid.NewGuid(), "Quiz", ActivityType.Quiz, true, ApprovalStrategy.PeerReview, 1);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Content_RejectsInvalidOrder()
    {
        var action = () => new Unit(Guid.NewGuid(), Guid.NewGuid(), "Unit 1", 0);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
}
