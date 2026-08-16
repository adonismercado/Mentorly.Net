using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Entities;

namespace Mentorly.Application.Services;

public sealed class PeerReviewService(
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    ISubmissionRepository submissionRepository,
    IPeerReviewRepository peerReviewRepository,
    IPeerReviewWorkflowRepository peerReviewWorkflowRepository,
    ICourseCompletionService courseCompletionService,
    IGamificationService gamificationService,
    IUnitOfWork unitOfWork) : IPeerReviewService
{
    public async Task<PeerReviewDto[]> GetAllPeerReviewsAsync(Guid adminId, CancellationToken cancellationToken = default)
    {
        await EnsureAdminAsync(adminId, cancellationToken);
        var peerReviews = await peerReviewRepository.GetAllAsync(cancellationToken);

        return peerReviews.Select(pr => new PeerReviewDto(
            pr.Id,
            pr.SubmissionId,
            pr.ReviewerStudentId,
            pr.IsApproved,
            pr.FeedbackComment,
            pr.CreatedAt))
            .ToArray();
    }

    public async Task<PeerReviewResultDto> SubmitReviewAsync(Guid reviewerStudentId, CreatePeerReviewRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!await studentRepository.ExistsAsync(reviewerStudentId, cancellationToken))
        {
            throw new InvalidOperationException("Reviewer student not found.");
        }

        var submission = await submissionRepository.GetByIdWithContextAsync(request.SubmissionId, cancellationToken)
            ?? throw new InvalidOperationException("Submission not found.");

        var activity = await peerReviewWorkflowRepository.GetActivityAsync(submission.ActivityId, cancellationToken)
            ?? throw new InvalidOperationException("Activity not found.");
        if (activity.ApprovalStrategy != Domain.Enums.ApprovalStrategy.PeerReview || submission.Status != Domain.Enums.SubmissionStatus.Pending)
        {
            throw new InvalidOperationException("Only pending peer-review submissions can be reviewed.");
        }

        if (submission.Enrollment.StudentId == reviewerStudentId)
        {
            throw new InvalidOperationException("Self-review is not allowed.");
        }

        var reviewerHasOwnSubmission = await submissionRepository.HasStudentSubmittedActivityAsync(
            reviewerStudentId,
            submission.ActivityId,
            cancellationToken);

        if (!reviewerHasOwnSubmission)
        {
            throw new InvalidOperationException("Reviewer must submit their own solution before reviewing peers.");
        }

        var alreadyReviewed = await peerReviewRepository.HasReviewerAlreadyReviewedAsync(
            submission.Id,
            reviewerStudentId,
            cancellationToken);

        if (alreadyReviewed)
        {
            throw new InvalidOperationException("The reviewer already reviewed this submission.");
        }

        await EnsureReviewerHasActiveEnrollmentAsync(reviewerStudentId, submission.Enrollment.CourseId, cancellationToken);

        var reviewedAtUtc = DateTime.UtcNow;
        var review = PeerReview.Create(
            request.SubmissionId,
            reviewerStudentId,
            request.IsApproved,
            request.FeedbackComment,
            reviewedAtUtc);

        await peerReviewRepository.AddAsync(review, cancellationToken);

        var positiveReviews = await peerReviewRepository.CountApprovalsForSubmissionAsync(submission.Id, cancellationToken);
        if (request.IsApproved)
        {
            positiveReviews++;
        }

        var requiredReviews = submission.Enrollment.Course.RequiredPeerReviews;

        var wasApproved = submission.Status == Domain.Enums.SubmissionStatus.Approved;
        if (positiveReviews >= requiredReviews)
        {
            submission.Approve(reviewedAtUtc);
        }
        // A negative peer review is feedback, not a final rejection. Only an administrator can reject definitively.

        await unitOfWork.SaveChangesAsync(cancellationToken);
        if (!wasApproved && submission.Status == Domain.Enums.SubmissionStatus.Approved)
        {
            await gamificationService.AwardAsync(submission.Enrollment.StudentId, Domain.Enums.GamificationEventType.ExerciseApproved, submission.Id, cancellationToken);
        }
        if (request.FeedbackComment.Trim().Length >= 20)
        {
            await gamificationService.AwardAsync(reviewerStudentId, Domain.Enums.GamificationEventType.ConstructivePeerReview, review.Id, cancellationToken);
        }
        await courseCompletionService.EvaluateAsync(submission.EnrollmentId, cancellationToken);

        return new PeerReviewResultDto(
            review.Id,
            review.SubmissionId,
            review.ReviewerStudentId,
            review.IsApproved,
            review.FeedbackComment,
            review.CreatedAt,
            positiveReviews,
            requiredReviews,
            submission.Status);
    }

    public async Task<ReviewQueueItemDto[]> GetEligibleQueueAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default)
    {
        if (!await studentRepository.ExistsAsync(reviewerStudentId, cancellationToken))
        {
            throw new InvalidOperationException("Reviewer student not found.");
        }

        var queue = await peerReviewWorkflowRepository.GetEligibleQueueAsync(reviewerStudentId, cancellationToken);
        return queue.Select(x => new ReviewQueueItemDto(x.SubmissionId, x.ActivityId, x.ActivityTitle, x.EvidenceType, x.EvidenceContent, x.SubmittedAtUtc)).ToArray();
    }

    public async Task<PeerReviewAuditDto?> GetAuditAsync(Guid adminId, Guid peerReviewId, CancellationToken cancellationToken = default)
    {
        await EnsureAdminAsync(adminId, cancellationToken);
        var audit = await peerReviewWorkflowRepository.GetAuditAsync(peerReviewId, cancellationToken);
        return audit is null ? null : new PeerReviewAuditDto(audit.PeerReviewId, audit.SubmissionId, audit.AuthorStudentId, audit.ReviewerStudentId, audit.IsApproved, audit.FeedbackComment, audit.CreatedAtUtc, audit.EvidenceType, audit.EvidenceContent);
    }

    public async Task<PeerReviewDto[]> GetMyPeerReviewsAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default)
    {
        return (await peerReviewRepository.GetByReviewerStudentIdAsync(reviewerStudentId, cancellationToken)).Select(pr => new PeerReviewDto(pr.Id, pr.SubmissionId, pr.ReviewerStudentId, pr.IsApproved, pr.FeedbackComment, pr.CreatedAt)).ToArray();
    }

    public async Task<AnonymousSubmissionDto?> GetAnonymousSubmissionAsync(Guid submissionId, Guid reviewerStudentId, CancellationToken cancellationToken = default)
    {
        var submission = await peerReviewWorkflowRepository.GetAnonymousSubmissionAsync(submissionId, reviewerStudentId, cancellationToken);
        return submission is null ? null : new AnonymousSubmissionDto(submission.SubmissionId, submission.ActivityId, submission.ActivityTitle, submission.EvidenceType, submission.EvidenceContent, submission.SubmittedAtUtc);
    }

    public async Task<bool> UpdatePeerReviewAsync(Guid peerReviewId, UpdatePeerReviewDto dto, CancellationToken cancellationToken = default)
    {
        var peerReview = await peerReviewRepository.GetByIdAsync(peerReviewId, cancellationToken);

        if (peerReview is null)
        {
            return false;
        }

        peerReview.UpdateReview(dto.IsApproved, dto.FeedbackComment);

        peerReviewRepository.Update(peerReview);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeletePeerReviewAsync(Guid peerReviewId, CancellationToken cancellationToken = default)
    {
        var peerReview = await peerReviewRepository.GetByIdAsync(peerReviewId, cancellationToken);

        if (peerReview is null)
        {
            return false;
        }

        peerReviewRepository.Delete(peerReview);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task EnsureAdminAsync(Guid adminId, CancellationToken cancellationToken)
    {
        var admin = await studentRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin?.Role != Domain.Enums.StudentRole.Admin)
        {
            throw new InvalidOperationException("Only an administrator can access peer-review administration.");
        }
    }

    private async Task EnsureReviewerHasActiveEnrollmentAsync(Guid reviewerStudentId, Guid courseId, CancellationToken cancellationToken)
    {
        var enrollments = await enrollmentRepository.GetByStudentIdAsync(reviewerStudentId, cancellationToken);
        if (!enrollments.Any(enrollment => enrollment.CourseId == courseId && enrollment.CanSubmit(DateTime.UtcNow)))
        {
            throw new InvalidOperationException("Reviewer must have an active enrollment in this course.");
        }
    }
}
