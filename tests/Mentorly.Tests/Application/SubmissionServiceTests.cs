using System.Reflection;
using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Tests.Application;

public sealed class SubmissionServiceTests
{
    [Fact]
    public async Task EscalateAsync_EscalatesRejectedPeerReviewSubmission()
    {
        var studentId = Guid.NewGuid();
        var enrollment = Enrollment.CreateNew(studentId, Guid.NewGuid(), 1, DateTime.UtcNow);
        var submission = Submission.Create(enrollment.Id, Guid.NewGuid(), EvidenceType.Url, "https://github.com/example/repository", DateTime.UtcNow);
        submission.Reject(DateTime.UtcNow);
        SetPrivateProperty(submission, nameof(Submission.Enrollment), enrollment);

        var service = new SubmissionService(
            new FakeSubmissionRepository(submission),
            new FakePeerReviewRepository(),
            new FakeEnrollmentRepository(),
            new FakeStudentRepository(),
            new FakePeerReviewWorkflowRepository(submission.ActivityId),
            new FakeCourseCompletionService(),
            new FakeGamificationService(),
            new FakeUnitOfWork());

        var result = await service.EscalateAsync(submission.Id, studentId);

        Assert.True(result);
        Assert.Equal(SubmissionStatus.Escalated, submission.Status);
    }

    [Fact]
    public async Task EscalateAsync_RejectsPendingSubmissionBeforeSeventyTwoHours()
    {
        var studentId = Guid.NewGuid();
        var enrollment = Enrollment.CreateNew(studentId, Guid.NewGuid(), 1, DateTime.UtcNow);
        var submission = Submission.Create(enrollment.Id, Guid.NewGuid(), EvidenceType.Url, "https://github.com/example/repository", DateTime.UtcNow.AddHours(-71));
        SetPrivateProperty(submission, nameof(Submission.Enrollment), enrollment);

        var service = new SubmissionService(
            new FakeSubmissionRepository(submission), new FakePeerReviewRepository(), new FakeEnrollmentRepository(),
            new FakeStudentRepository(), new FakePeerReviewWorkflowRepository(submission.ActivityId),
            new FakeCourseCompletionService(), new FakeGamificationService(), new FakeUnitOfWork());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.EscalateAsync(submission.Id, studentId));

        Assert.Contains("72 hours", exception.Message);
        Assert.Equal(SubmissionStatus.Pending, submission.Status);
    }

    [Fact]
    public async Task EscalateAsync_EscalatesPendingSubmissionAfterSeventyTwoHours()
    {
        var studentId = Guid.NewGuid();
        var enrollment = Enrollment.CreateNew(studentId, Guid.NewGuid(), 1, DateTime.UtcNow);
        var submission = Submission.Create(enrollment.Id, Guid.NewGuid(), EvidenceType.Url, "https://github.com/example/repository", DateTime.UtcNow.AddHours(-73));
        SetPrivateProperty(submission, nameof(Submission.Enrollment), enrollment);

        var service = new SubmissionService(
            new FakeSubmissionRepository(submission), new FakePeerReviewRepository(), new FakeEnrollmentRepository(),
            new FakeStudentRepository(), new FakePeerReviewWorkflowRepository(submission.ActivityId),
            new FakeCourseCompletionService(), new FakeGamificationService(), new FakeUnitOfWork());

        var result = await service.EscalateAsync(submission.Id, studentId);

        Assert.True(result);
        Assert.Equal(SubmissionStatus.Escalated, submission.Status);
    }

    [Fact]
    public async Task GetEscalatedSubmissionsAsync_ReturnsAdministrativeContextForAdmin()
    {
        var admin = new Student(Guid.NewGuid(), "admin-google-id", "admin@mentorly.com", "Admin Mentorly");
        admin.PromoteToAdmin();
        var queueItem = new AdminEscalatedSubmissionData(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Estudiante Mentorly",
            Guid.NewGuid(),
            "Curso de Android",
            Guid.NewGuid(),
            "Ejercicio Compose",
            EvidenceType.Url,
            "https://github.com/student/compose",
            DateTime.UtcNow.AddDays(-2),
            DateTime.UtcNow.AddDays(-1),
            2,
            1);
        var submission = Submission.Create(Guid.NewGuid(), queueItem.ActivityId, queueItem.EvidenceType, queueItem.EvidenceContent, queueItem.SubmittedAtUtc);

        var service = new SubmissionService(
            new FakeSubmissionRepository(submission, [queueItem]),
            new FakePeerReviewRepository(),
            new FakeEnrollmentRepository(),
            new FakeStudentRepository(admin),
            new FakePeerReviewWorkflowRepository(submission.ActivityId),
            new FakeCourseCompletionService(),
            new FakeGamificationService(),
            new FakeUnitOfWork());

        var result = await service.GetEscalatedSubmissionsAsync(admin.Id);

        var item = Assert.Single(result);
        Assert.Equal(queueItem.SubmissionId, item.SubmissionId);
        Assert.Equal(queueItem.AuthorDisplayName, item.AuthorDisplayName);
        Assert.Equal(queueItem.CourseTitle, item.CourseTitle);
        Assert.Equal(queueItem.PositiveReviews, item.PositiveReviews);
        Assert.Equal(queueItem.RejectedReviews, item.RejectedReviews);
    }

    [Fact]
    public async Task GetEscalatedSubmissionAuditAsync_ReturnsAuthorAndReviewerIdentitiesForAdmin()
    {
        var admin = new Student(Guid.NewGuid(), "admin-google-id", "admin@mentorly.com", "Admin Mentorly");
        admin.PromoteToAdmin();
        var review = new AdminPeerReviewAuditItemData(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Revisor Mentorly",
            "reviewer@mentorly.com",
            true,
            "La evidencia cumple con los requisitos.",
            DateTime.UtcNow.AddHours(-4));
        var audit = new AdminSubmissionAuditData(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Autora Mentorly",
            "author@mentorly.com",
            Guid.NewGuid(),
            "Curso de Android",
            Guid.NewGuid(),
            "Ejercicio Compose",
            EvidenceType.Url,
            "https://github.com/author/compose",
            SubmissionStatus.Escalated,
            DateTime.UtcNow.AddDays(-2),
            DateTime.UtcNow.AddDays(-1),
            [review]);
        var submission = Submission.Create(audit.EnrollmentId, audit.ActivityId, audit.EvidenceType, audit.EvidenceContent, audit.SubmittedAtUtc);

        var service = new SubmissionService(
            new FakeSubmissionRepository(submission, audit: audit),
            new FakePeerReviewRepository(),
            new FakeEnrollmentRepository(),
            new FakeStudentRepository(admin),
            new FakePeerReviewWorkflowRepository(submission.ActivityId),
            new FakeCourseCompletionService(),
            new FakeGamificationService(),
            new FakeUnitOfWork());

        var result = await service.GetEscalatedSubmissionAuditAsync(admin.Id, audit.SubmissionId);

        Assert.NotNull(result);
        Assert.Equal(audit.AuthorEmail, result.AuthorEmail);
        Assert.Equal(audit.CourseTitle, result.CourseTitle);
        var auditReview = Assert.Single(result.PeerReviews);
        Assert.Equal(review.ReviewerEmail, auditReview.ReviewerEmail);
        Assert.Equal(review.FeedbackComment, auditReview.FeedbackComment);
    }

    [Fact]
    public async Task DecideAsAdminAsync_ApprovesEscalatedPeerReviewSubmission()
    {
        var admin = new Student(Guid.NewGuid(), "admin-google-id", "admin@mentorly.com", "Admin Mentorly");
        admin.PromoteToAdmin();
        var enrollment = Enrollment.CreateNew(Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.UtcNow);
        var submission = Submission.Create(enrollment.Id, Guid.NewGuid(), EvidenceType.Url, "https://github.com/student/repository", DateTime.UtcNow);
        submission.Escalate(DateTime.UtcNow);
        SetPrivateProperty(submission, nameof(Submission.Enrollment), enrollment);

        var service = new SubmissionService(
            new FakeSubmissionRepository(submission),
            new FakePeerReviewRepository(),
            new FakeEnrollmentRepository(),
            new FakeStudentRepository(admin),
            new FakePeerReviewWorkflowRepository(submission.ActivityId),
            new FakeCourseCompletionService(),
            new FakeGamificationService(),
            new FakeUnitOfWork());

        var result = await service.DecideAsAdminAsync(admin.Id, submission.Id, isApproved: true);

        Assert.True(result);
        Assert.Equal(SubmissionStatus.Approved, submission.Status);
    }

    private static void SetPrivateProperty<TTarget, TValue>(TTarget target, string propertyName, TValue value)
        where TTarget : class
    {
        var property = typeof(TTarget).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found.");

        property.SetValue(target, value);
    }

    private sealed class FakeSubmissionRepository(
        Submission submission,
        AdminEscalatedSubmissionData[]? escalatedSubmissions = null,
        AdminSubmissionAuditData? audit = null) : ISubmissionRepository
    {
        public Task<Submission[]> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<Submission[]>([]);
        public Task<AdminEscalatedSubmissionData[]> GetEscalatedForAdminAsync(CancellationToken cancellationToken = default) => Task.FromResult(escalatedSubmissions ?? []);
        public Task<AdminSubmissionAuditData?> GetEscalatedAuditAsync(Guid submissionId, CancellationToken cancellationToken = default) => Task.FromResult(audit?.SubmissionId == submissionId ? audit : null);
        public Task<Submission?> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken = default) => Task.FromResult<Submission?>(submissionId == submission.Id ? submission : null);
        public Task<Submission?> GetByIdWithContextAsync(Guid submissionId, CancellationToken cancellationToken = default) => Task.FromResult<Submission?>(submissionId == submission.Id ? submission : null);
        public Task<Submission?> GetByEnrollmentAndActivityAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken = default) => Task.FromResult<Submission?>(null);
        public Task<bool> HasStudentSubmittedActivityAsync(Guid studentId, Guid activityId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> HasSubmissionsForActivityAsync(Guid activityId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<IReadOnlySet<Guid>> GetApprovedActivityIdsAsync(Guid enrollmentId, IReadOnlyCollection<Guid> activityIds, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlySet<Guid>>(new HashSet<Guid>());
        public Task<Submission[]> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult<Submission[]>([]);
        public Task AddAsync(Submission submission, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task DeleteAsync(Submission submission, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakePeerReviewRepository : IPeerReviewRepository
    {
        public Task<PeerReview[]> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<PeerReview[]>([]);
        public Task<PeerReview?> GetByIdAsync(Guid peerReviewId, CancellationToken cancellationToken = default) => Task.FromResult<PeerReview?>(null);
        public Task<PeerReview[]> GetBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken = default) => Task.FromResult<PeerReview[]>([]);
        public Task<PeerReview[]> GetByReviewerStudentIdAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default) => Task.FromResult<PeerReview[]>([]);
        public Task<bool> HasReviewerAlreadyReviewedAsync(Guid submissionId, Guid reviewerStudentId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<int> CountApprovalsForSubmissionAsync(Guid submissionId, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task AddAsync(PeerReview review, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Update(PeerReview review) { }
        public void Delete(PeerReview review) { }
    }

    private sealed class FakeEnrollmentRepository : IEnrollmentRepository
    {
        public Task<Enrollment[]> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<Enrollment[]>([]);
        public Task<Enrollment?> GetByIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default) => Task.FromResult<Enrollment?>(null);
        public Task<Enrollment[]> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult<Enrollment[]>([]);
        public Task<Enrollment?> GetLatestByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default) => Task.FromResult<Enrollment?>(null);
        public Task<bool> HasActiveEnrollmentAsync(Guid studentId, Guid courseId, DateTime utcNow, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<int> GetNextAttemptNumberAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default) => Task.FromResult(1);
        public Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Add(Enrollment enrollment) { }
    }

    private sealed class FakeStudentRepository(Student? student = null) : IStudentRepository
    {
        public Task<Student?> GetByIdWithBadgesAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult<Student?>(null);
        public Task<bool> ExistsAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<Student?> GetByIdAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult(student?.Id == studentId ? student : null);
        public Task<Student[]> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<Student[]>([]);
        public void Add(Student student) { }
        public void Update(Student student) { }
        public void Delete(Student student) { }
    }

    private sealed class FakePeerReviewWorkflowRepository(Guid activityId) : IPeerReviewWorkflowRepository
    {
        public Task<ActivityWorkflowData?> GetActivityAsync(Guid requestedActivityId, CancellationToken cancellationToken = default)
            => Task.FromResult<ActivityWorkflowData?>(requestedActivityId == activityId
                ? new ActivityWorkflowData(activityId, Guid.NewGuid(), Guid.NewGuid(), 1, ActivityType.Exercise, true, ApprovalStrategy.PeerReview, 1)
                : null);

        public Task<bool> CanSubmitMandatoryActivityAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task<IReadOnlyList<ReviewQueueItemData>> GetEligibleQueueAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ReviewQueueItemData>>([]);
        public Task<ReviewAuditData?> GetAuditAsync(Guid peerReviewId, CancellationToken cancellationToken = default) => Task.FromResult<ReviewAuditData?>(null);
        public Task<AnonymousSubmissionData?> GetAnonymousSubmissionAsync(Guid submissionId, Guid reviewerStudentId, CancellationToken cancellationToken = default) => Task.FromResult<AnonymousSubmissionData?>(null);
    }

    private sealed class FakeCourseCompletionService : ICourseCompletionService
    {
        public Task<EnrollmentProgressDto?> EvaluateAsync(Guid enrollmentId, CancellationToken cancellationToken = default) => Task.FromResult<EnrollmentProgressDto?>(null);
    }

    private sealed class FakeGamificationService : IGamificationService
    {
        public Task AwardAsync(Guid studentId, GamificationEventType type, Guid referenceId, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }
}
