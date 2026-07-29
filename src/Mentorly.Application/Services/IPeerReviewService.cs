using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface IPeerReviewService
{
    Task<PeerReviewResultDto> SubmitReviewAsync(CreatePeerReviewRequestDto request, CancellationToken cancellationToken = default);
}
