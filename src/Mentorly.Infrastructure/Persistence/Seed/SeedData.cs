namespace Mentorly.Infrastructure.Persistence.Seed;

public static class SeedData
{
    public static readonly Guid StudentId = Guid.Parse("f43f2c2f-2db4-47cd-8a42-7b0f3c495601");
    public static readonly Guid ReviewerStudentId = Guid.Parse("b7e670c1-caf3-4da5-a8f7-34570fbb9d41");
    public static readonly Guid AdminId = Guid.Parse("80bbec34-8a28-4e38-ab64-92662f0b5b5b");
    public static readonly Guid CourseId = Guid.Parse("cb57a2a9-aa8e-4538-aa86-d8e383136fdc");
    public static readonly DateTime CourseCreatedAtUtc = new(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc);
    public static readonly Guid ActivityId = Guid.Parse("f3af6a42-266d-4468-b840-f26e95ec6e6b");
    public static readonly Guid UnitId = Guid.Parse("be480fd4-6392-4a0d-91fd-5a3e773e9c10");
    public static readonly Guid ThemeId = Guid.Parse("a8466ce6-95c6-4d7d-a998-38925240cd70");
    public static readonly Guid QuizActivityId = Guid.Parse("6a6538ef-9454-4a5d-80ac-344d8a4068de");
    public static readonly Guid CourseImageId = Guid.Parse("f74e10ed-86b4-47e5-8caf-d07af6cd2b25");
    public static readonly Guid SeedEnrollmentId = Guid.Parse("d9f7ebf1-6f9f-4b61-9870-86ae9be79cb1");
    public static readonly Guid SeedSubmissionId = Guid.Parse("9980b9e0-d0cc-42f5-bf54-e5f3fd56bc56");
    public static readonly Guid AuthorEnrollmentId = Guid.Parse("b82acd0a-9bd4-4e5e-b2d9-01e3283285f1");
    public static readonly Guid AuthorSubmissionId = Guid.Parse("a1904ac6-c334-4126-9f2f-03dd9a6276e6");
    public static readonly Guid SeedPeerReviewId = Guid.Parse("1f3c9c12-c628-4d29-9887-271c4cd71fe0");
    public static readonly DateTime SeedStartedAtUtc = new(2026, 01, 05, 0, 0, 0, DateTimeKind.Utc);
    public static readonly DateTime SeedSubmittedAtUtc = new(2026, 01, 06, 0, 0, 0, DateTimeKind.Utc);
}
