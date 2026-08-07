using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface IStudentService
{
    Task<IReadOnlyList<StudentDto>> GetAllStudentsAsync(CancellationToken cancellationToken = default);
    Task<StudentDto?> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<StudentDto> CreateStudentAsync(CreateStudentDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateStudentAsync(Guid studentId, UpdateStudentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<bool> UpdateLeaderboardPrivacyAsync(Guid studentId, bool isLeaderboardPublic, CancellationToken cancellationToken = default);
    Task<StudentStatisticsDto?> GetStudentStatisticsAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<bool> PromoteToAdminAsync(Guid studentId, CancellationToken cancellationToken = default);
}
