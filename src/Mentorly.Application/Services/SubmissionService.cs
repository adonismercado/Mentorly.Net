using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Application.Services;

public sealed class SubmissionService(
    ISubmissionRepository submissionRepository,
    IPeerReviewRepository peerReviewRepository,
    IEnrollmentRepository enrollmentRepository,
    IStudentRepository studentRepository,
    IPeerReviewWorkflowRepository peerReviewWorkflowRepository,
    ICourseCompletionService courseCompletionService,
    IGamificationService gamificationService,
    IUnitOfWork unitOfWork) : ISubmissionService
{
    public async Task<SubmissionDto[]> GetAllSubmissionsAsync(CancellationToken cancellationToken = default)
    {
        var submissions = await submissionRepository.GetAllAsync(cancellationToken);

        return submissions.Select(s => new SubmissionDto(
            s.Id,
            s.EnrollmentId,
            s.ActivityId,
            s.Activity.Title,
            s.EvidenceType,
            s.EvidenceContent,
            s.Status,
            s.SubmittedAt,
            s.ReviewedAt))
            .ToArray();
    }

    public async Task<AdminEscalatedSubmissionDto[]> GetEscalatedSubmissionsAsync(
        Guid adminId,
        CancellationToken cancellationToken = default)
    {
        var admin = await studentRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin?.Role != StudentRole.Admin)
        {
            throw new InvalidOperationException("Only an administrator can view escalated submissions.");
        }

        var submissions = await submissionRepository.GetEscalatedForAdminAsync(cancellationToken);
        return submissions.Select(submission => new AdminEscalatedSubmissionDto(
            submission.SubmissionId,
            submission.EnrollmentId,
            submission.AuthorStudentId,
            submission.AuthorDisplayName,
            submission.CourseId,
            submission.CourseTitle,
            submission.ActivityId,
            submission.ActivityTitle,
            submission.EvidenceType,
            submission.EvidenceContent,
            submission.SubmittedAtUtc,
            submission.EscalatedAtUtc,
            submission.PositiveReviews,
            submission.RejectedReviews)).ToArray();
    }

    public async Task<AdminSubmissionAuditDto?> GetEscalatedSubmissionAuditAsync(
        Guid adminId,
        Guid submissionId,
        CancellationToken cancellationToken = default)
    {
        var admin = await studentRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin?.Role != StudentRole.Admin)
        {
            throw new InvalidOperationException("Only an administrator can audit escalated submissions.");
        }

        var audit = await submissionRepository.GetEscalatedAuditAsync(submissionId, cancellationToken);
        if (audit is null)
        {
            return null;
        }

        return new AdminSubmissionAuditDto(
            audit.SubmissionId,
            audit.EnrollmentId,
            audit.AuthorStudentId,
            audit.AuthorDisplayName,
            audit.AuthorEmail,
            audit.CourseId,
            audit.CourseTitle,
            audit.ActivityId,
            audit.ActivityTitle,
            audit.EvidenceType,
            audit.EvidenceContent,
            audit.Status,
            audit.SubmittedAtUtc,
            audit.ReviewedAtUtc,
            audit.PeerReviews.Select(review => new AdminPeerReviewAuditItemDto(
                review.PeerReviewId,
                review.ReviewerStudentId,
                review.ReviewerDisplayName,
                review.ReviewerEmail,
                review.IsApproved,
                review.FeedbackComment,
                review.CreatedAtUtc)).ToArray());
    }

    public async Task<SubmissionDto?> GetSubmissionByIdAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdAsync(submissionId, cancellationToken);

        if (submission is null)
        {
            return null;
        }

        return new SubmissionDto(
            submission.Id,
            submission.EnrollmentId,
            submission.ActivityId,
            submission.Activity.Title,
            submission.EvidenceType,
            submission.EvidenceContent,
            submission.Status,
            submission.SubmittedAt,
            submission.ReviewedAt);
    }

    public async Task<SubmissionDto> CreateSubmissionAsync(Guid enrollmentId, Guid activityId, CreateSubmissionDto dto, CancellationToken cancellationToken = default)
    {
        var enrollment = await GetActiveEnrollmentAsync(enrollmentId, cancellationToken);
        var activity = await ValidateActivityCanBeSubmittedAsync(enrollment, activityId, cancellationToken);
        var existingSubmission = await submissionRepository.GetByEnrollmentAndActivityAsync(enrollmentId, activityId, cancellationToken);
        if (existingSubmission is not null)
        {
            existingSubmission.ReplaceEvidence(dto.EvidenceType, dto.EvidenceContent);
            ApplyApprovalStrategy(existingSubmission, activity.ApprovalStrategy);
            await submissionRepository.UpdateAsync(existingSubmission, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await EvaluateIfApprovedAsync(existingSubmission, cancellationToken);
            return Map(existingSubmission);
        }

        var submission = Submission.Create(
            enrollmentId,
            activityId,
            dto.EvidenceType,
            dto.EvidenceContent,
            DateTime.UtcNow);

        ApplyApprovalStrategy(submission, activity.ApprovalStrategy);
        await submissionRepository.AddAsync(submission, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await gamificationService.AwardAsync(enrollment.StudentId, GamificationEventType.ExerciseSubmitted, submission.Id, cancellationToken);
        await EvaluateIfApprovedAsync(submission, cancellationToken);

        var createdSubmission = await submissionRepository.GetByIdAsync(submission.Id, cancellationToken)
            ?? submission;
        return Map(createdSubmission);
    }

    public async Task<bool> UpdateSubmissionAsync(Guid submissionId, UpdateSubmissionDto dto, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdWithContextAsync(submissionId, cancellationToken);

        if (submission is null)
        {
            return false;
        }

        var enrollment = await GetActiveEnrollmentAsync(submission.EnrollmentId, cancellationToken);
        var activity = await ValidateActivityCanBeSubmittedAsync(enrollment, submission.ActivityId, cancellationToken);

        submission.ReplaceEvidence(dto.EvidenceType, dto.EvidenceContent);
        ApplyApprovalStrategy(submission, activity.ApprovalStrategy);

        await submissionRepository.UpdateAsync(submission, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await EvaluateIfApprovedAsync(submission, cancellationToken);

        return true;
    }

    public async Task<bool> DeleteSubmissionAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdAsync(submissionId, cancellationToken);

        if (submission is null)
        {
            return false;
        }

        await submissionRepository.DeleteAsync(submission, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> EscalateAsync(Guid submissionId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdWithContextAsync(submissionId, cancellationToken);
        if (submission is null || submission.Enrollment.StudentId != studentId)
        {
            return false;
        }

        var activity = await peerReviewWorkflowRepository.GetActivityAsync(submission.ActivityId, cancellationToken)
            ?? throw new InvalidOperationException("Activity not found.");
        var canBeEscalated = submission.Status is SubmissionStatus.Pending or SubmissionStatus.Rejected;
        if (activity.ApprovalStrategy != ApprovalStrategy.PeerReview || !canBeEscalated)
        {
            throw new InvalidOperationException("Only pending or rejected peer-review submissions can be escalated.");
        }

        submission.Escalate(DateTime.UtcNow);
        await submissionRepository.UpdateAsync(submission, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DecideAsAdminAsync(Guid adminId, Guid submissionId, bool isApproved, CancellationToken cancellationToken = default)
    {
        var admin = await studentRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin?.Role != StudentRole.Admin)
        {
            throw new InvalidOperationException("Only an administrator can decide a submission.");
        }

        var submission = await submissionRepository.GetByIdWithContextAsync(submissionId, cancellationToken);
        if (submission is null)
        {
            return false;
        }

        var activity = await peerReviewWorkflowRepository.GetActivityAsync(submission.ActivityId, cancellationToken)
            ?? throw new InvalidOperationException("Activity not found.");
        var isEscalatedPeerReviewSubmission =
            activity.ApprovalStrategy == ApprovalStrategy.PeerReview &&
            submission.Status == SubmissionStatus.Escalated;

        if (!isEscalatedPeerReviewSubmission)
        {
            throw new InvalidOperationException("Only escalated peer-review submissions can receive an administrative decision.");
        }

        if (isApproved)
        {
            submission.Approve(DateTime.UtcNow);
        }
        else
        {
            submission.Reject(DateTime.UtcNow);
        }

        await submissionRepository.UpdateAsync(submission, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        if (isApproved)
        {
            await gamificationService.AwardAsync(submission.Enrollment.StudentId, Domain.Enums.GamificationEventType.ExerciseApproved, submission.Id, cancellationToken);
        }
        await courseCompletionService.EvaluateAsync(submission.EnrollmentId, cancellationToken);
        return true;
    }

    public async Task<SubmissionDto[]> GetMySubmissionsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return (await submissionRepository.GetByStudentIdAsync(studentId, cancellationToken))
            .Select(submission => Map(submission))
            .ToArray();
    }

    public async Task<PeerReviewFeedbackDto[]?> GetMySubmissionReviewsAsync(Guid submissionId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdWithContextAsync(submissionId, cancellationToken);
        if (submission is null || submission.Enrollment.StudentId != studentId) return null;
        return (await peerReviewRepository.GetBySubmissionIdAsync(submissionId, cancellationToken)).Select(x => new PeerReviewFeedbackDto(x.Id, x.IsApproved, x.FeedbackComment, x.CreatedAt)).ToArray();
    }

    private static SubmissionDto Map(Submission submission) => new(
        submission.Id,
        submission.EnrollmentId,
        submission.ActivityId,
        submission.Activity?.Title ?? string.Empty,
        submission.EvidenceType,
        submission.EvidenceContent,
        submission.Status,
        submission.SubmittedAt,
        submission.ReviewedAt);

    private async Task<Enrollment> GetActiveEnrollmentAsync(Guid enrollmentId, CancellationToken cancellationToken)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken)
            ?? throw new InvalidOperationException("Enrollment not found.");

        if (!enrollment.CanSubmit(DateTime.UtcNow))
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException("Enrollment is expired or inactive. Submission is not allowed.");
        }

        return enrollment;
    }

    private async Task<ActivityWorkflowData> ValidateActivityCanBeSubmittedAsync(Enrollment enrollment, Guid activityId, CancellationToken cancellationToken)
    {
        var activity = await peerReviewWorkflowRepository.GetActivityAsync(activityId, cancellationToken)
            ?? throw new InvalidOperationException("Activity not found.");

        if (activity.CourseId != enrollment.CourseId)
        {
            throw new InvalidOperationException("The activity does not belong to the enrollment course.");
        }

        if (!await peerReviewWorkflowRepository.CanSubmitMandatoryActivityAsync(enrollment.Id, activityId, cancellationToken))
        {
            throw new InvalidOperationException("Previous mandatory exercises must be approved and the peer-review quota completed before submitting this unit.");
        }

        return activity;
    }

    private static void ApplyApprovalStrategy(Submission submission, ApprovalStrategy approvalStrategy)
    {
        if (approvalStrategy == ApprovalStrategy.Auto)
        {
            submission.Approve(DateTime.UtcNow);
        }
    }

    private async Task EvaluateIfApprovedAsync(Submission submission, CancellationToken cancellationToken)
    {
        if (submission.Status != SubmissionStatus.Approved)
        {
            return;
        }

        var enrollment = await enrollmentRepository.GetByIdAsync(submission.EnrollmentId, cancellationToken)
            ?? throw new InvalidOperationException("Enrollment not found.");

        await gamificationService.AwardAsync(enrollment.StudentId, GamificationEventType.ExerciseApproved, submission.Id, cancellationToken);
        await courseCompletionService.EvaluateAsync(submission.EnrollmentId, cancellationToken);
    }
}
