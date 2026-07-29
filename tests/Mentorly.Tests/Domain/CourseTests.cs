using Mentorly.Domain.Entities;

namespace Mentorly.Tests.Domain;

public sealed class CourseTests
{
    [Fact]
    public void Constructor_Throws_WhenRequiredPeerReviewsIsInvalid()
    {
        var action = () => new Course(
            Guid.NewGuid(),
            "Clean Architecture",
            "Course description",
            Guid.NewGuid(),
            0);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void UpdateRequiredPeerReviews_Updates_WhenValueIsValid()
    {
        var course = new Course(
            Guid.NewGuid(),
            "Clean Architecture",
            "Course description",
            Guid.NewGuid(),
            1);

        course.UpdateRequiredPeerReviews(3);

        Assert.Equal(3, course.RequiredPeerReviews);
    }
}
