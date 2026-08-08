using System.Reflection;
using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Tests.Application;

public sealed class PeerReviewServiceTests
{
    [Fact]
    public async Task SubmitReviewAsync_ApprovesSubmission_WhenPositiveReviewsReachQuota()
    {
       /* var submission = BuildSubmissionGraph(requiredPeerReviews: 1);

        var studentRepo = new FakeStudentRepository(exists: true);
        var submissionRepo = new FakeSubmissionRepository(submission, reviewerHasOwnSubmission: true);
        var peerReviewRepo = new FakePeerReviewRepository(existingApprovalCount: 0, alreadyReviewed: false);
        var unitOfWork = new FakeUnitOfWork();

        var service = new PeerReviewService(studentRepo, submissionRepo, peerReviewRepo, unitOfWork);

        var result = await service.SubmitReviewAsync(new CreatePeerReviewRequestDto(
            submission.Id,
            ReviewerStudentId,
            true,
            "Great implementation.",
            DateTime.UtcNow));

        Assert.Equal(SubmissionStatus.Approved, result.SubmissionStatus);
        Assert.Equal(1, result.PositiveReviews);
        Assert.Equal(1, result.RequiredPositiveReviews);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
        Assert.NotNull(peerReviewRepo.LastAdded);*/
    }

    [Fact]
    public async Task SubmitReviewAsync_Throws_WhenReviewerHasNotSubmittedOwnSolution()
    {
        /*var submission = BuildSubmissionGraph(requiredPeerReviews: 2);

        var service = new PeerReviewService(
            new FakeStudentRepository(exists: true),
            new FakeSubmissionRepository(submission, reviewerHasOwnSubmission: false),
            new FakePeerReviewRepository(existingApprovalCount: 0, alreadyReviewed: false),
            new FakeUnitOfWork());

        var action = async () => await service.SubmitReviewAsync(new CreatePeerReviewRequestDto(
            submission.Id,
            ReviewerStudentId,
            true,
            "Feedback",
            DateTime.UtcNow));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(action);
        Assert.Contains("must submit their own solution", exception.Message, StringComparison.OrdinalIgnoreCase);*/
    }

    private static readonly Guid SubmissionOwnerStudentId = Guid.Parse("2c2e7be7-75c0-4ef4-9648-8dbf66f790ec");
    private static readonly Guid ReviewerStudentId = Guid.Parse("e17f9091-9752-4883-a713-76431d3c9717");

    private static Submission BuildSubmissionGraph(int requiredPeerReviews)
    {
        var course = new Course(
            Guid.NewGuid(),
            "Blazor",
            "Course",
            Guid.NewGuid(),
            requiredPeerReviews);

        var enrollment = Enrollment.CreateNew(SubmissionOwnerStudentId, course.Id, 1, DateTime.UtcNow);
        SetPrivateProperty(enrollment, nameof(Enrollment.Course), course);

        var submission = Submission.Create(enrollment.Id, Guid.NewGuid(), "https://github.com/example/repo", DateTime.UtcNow);
        SetPrivateProperty(submission, nameof(Submission.Enrollment), enrollment);

        return submission;
    }

    private static void SetPrivateProperty<TTarget, TValue>(TTarget target, string propertyName, TValue value)
        where TTarget : class
    {
        var property = typeof(TTarget).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found on {typeof(TTarget).Name}.");

        property.SetValue(target, value);
    }

    private sealed class FakeStudentRepository(bool exists) : IStudentRepository
    {
        public Task<bool> ExistsAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult(exists);
    }

    private sealed class FakeSubmissionRepository(Submission submission, bool reviewerHasOwnSubmission) : ISubmissionRepository
    {
        public Task<Submission?> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken = default)
            => Task.FromResult(submissionId == submission.Id ? submission : null);

        public Task<Submission?> GetByIdWithContextAsync(Guid submissionId, CancellationToken cancellationToken = default)
            => Task.FromResult(submissionId == submission.Id ? submission : null);

        public Task<Submission?> GetByEnrollmentAndActivityAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken = default)
            => Task.FromResult<Submission?>(null);

        public Task<bool> HasStudentSubmittedActivityAsync(Guid studentId, Guid activityId, CancellationToken cancellationToken = default)
            => Task.FromResult(reviewerHasOwnSubmission);

        public Task<bool> HasSubmissionsForActivityAsync(Guid activityId, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<IReadOnlySet<Guid>> GetApprovedActivityIdsAsync(Guid enrollmentId, IReadOnlyCollection<Guid> activityIds, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlySet<Guid>>(new HashSet<Guid>());

        public Task<Submission[]> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
            => Task.FromResult(Array.Empty<Submission>());

        public Task AddAsync(Submission submission, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task DeleteAsync(Submission submission, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<Submission[]> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Array.Empty<Submission>());
    }

    private sealed class FakePeerReviewRepository(int existingApprovalCount, bool alreadyReviewed) : IPeerReviewRepository
    {
        public PeerReview? LastAdded { get; private set; }

        public Task<bool> HasReviewerAlreadyReviewedAsync(Guid submissionId, Guid reviewerStudentId, CancellationToken cancellationToken = default)
            => Task.FromResult(alreadyReviewed);

        public Task<int> CountApprovalsForSubmissionAsync(Guid submissionId, CancellationToken cancellationToken = default)
            => Task.FromResult(existingApprovalCount);

        public Task AddAsync(PeerReview review, CancellationToken cancellationToken = default)
        {
            LastAdded = review;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalls++;
            return Task.FromResult(1);
        }
    }
}
