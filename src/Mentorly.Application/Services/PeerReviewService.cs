using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Entities;

namespace Mentorly.Application.Services;

public sealed class PeerReviewService(
    IStudentRepository studentRepository,
    ISubmissionRepository submissionRepository,
    IPeerReviewRepository peerReviewRepository,
    IUnitOfWork unitOfWork) : IPeerReviewService
{
    public async Task<IReadOnlyList<PeerReviewDto>> GetAllPeerReviewsAsync(CancellationToken cancellationToken = default)
    {
        var peerReviews = await peerReviewRepository.GetAllAsync(cancellationToken);

        return peerReviews.Select(pr => new PeerReviewDto(
            pr.Id,
            pr.SubmissionId,
            pr.ReviewerStudentId,
            pr.IsApproved,
            pr.FeedbackComment,
            pr.CreatedAt))
            .ToList();
    }

    public async Task<PeerReviewDto?> GetPeerReviewByIdAsync(Guid peerReviewId, CancellationToken cancellationToken = default)
    {
        var peerReview = await peerReviewRepository.GetByIdAsync(peerReviewId, cancellationToken);

        if (peerReview is null)
        {
            return null;
        }

        return new PeerReviewDto(
            peerReview.Id,
            peerReview.SubmissionId,
            peerReview.ReviewerStudentId,
            peerReview.IsApproved,
            peerReview.FeedbackComment,
            peerReview.CreatedAt);
    }

    public async Task<PeerReviewResultDto> SubmitReviewAsync(CreatePeerReviewRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!await studentRepository.ExistsAsync(request.ReviewerStudentId, cancellationToken))
        {
            throw new InvalidOperationException("Reviewer student not found.");
        }

        var submission = await submissionRepository.GetByIdWithContextAsync(request.SubmissionId, cancellationToken)
            ?? throw new InvalidOperationException("Submission not found.");

        if (submission.Enrollment.StudentId == request.ReviewerStudentId)
        {
            throw new InvalidOperationException("Self-review is not allowed.");
        }

        var reviewerHasOwnSubmission = await submissionRepository.HasStudentSubmittedActivityAsync(
            request.ReviewerStudentId,
            submission.ActivityId,
            cancellationToken);

        if (!reviewerHasOwnSubmission)
        {
            throw new InvalidOperationException("Reviewer must submit their own solution before reviewing peers.");
        }

        var alreadyReviewed = await peerReviewRepository.HasReviewerAlreadyReviewedAsync(
            submission.Id,
            request.ReviewerStudentId,
            cancellationToken);

        if (alreadyReviewed)
        {
            throw new InvalidOperationException("The reviewer already reviewed this submission.");
        }

        var review = PeerReview.Create(
            request.SubmissionId,
            request.ReviewerStudentId,
            request.IsApproved,
            request.FeedbackComment,
            request.CreatedAtUtc);

        await peerReviewRepository.AddAsync(review, cancellationToken);

        var positiveReviews = await peerReviewRepository.CountApprovalsForSubmissionAsync(submission.Id, cancellationToken);
        if (request.IsApproved)
        {
            positiveReviews++;
        }

        var requiredReviews = submission.Enrollment.Course.RequiredPeerReviews;

        if (positiveReviews >= requiredReviews)
        {
            submission.Approve(request.CreatedAtUtc);
        }
        else if (!request.IsApproved)
        {
            submission.Reject(request.CreatedAtUtc);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

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
}
