namespace Mentorly.Application.DTOs;

public sealed record CreateStudentDto(
    string GoogleUserId,
    string Email,
    string DisplayName);
