namespace Mentorly.Application.DTOs;

public sealed record ProvisionStudentDto(
    string GoogleUserId,
    string Email,
    string DisplayName);
