using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record EnrollmentProgressDto(
    Guid EnrollmentId,
    EnrollmentStatus Status,
    DateTime StartedAtUtc,
    DateTime ExpiresAtUtc,
    int TotalThemes,
    int CompletedThemes,
    int TotalMandatoryActivities,
    int ApprovedMandatoryActivities,
    int Percentage,
    bool IsCompleted,
    string? CertificateUrl,
    bool CanSubmitNextUnit,
    string? BlockedReason,
    EnrollmentUnitProgressDto[] Units);

public sealed record EnrollmentUnitProgressDto(
    Guid UnitId,
    string Title,
    int CompletedThemes,
    int TotalThemes,
    int ApprovedMandatoryActivities,
    int TotalMandatoryActivities,
    EnrollmentThemeProgressDto[] Themes);

public sealed record EnrollmentThemeProgressDto(
    Guid ThemeId,
    string Title,
    string ContentText,
    int OrderIndex,
    bool IsCompleted,
    EnrollmentActivityProgressDto[] Activities);

public sealed record EnrollmentActivityProgressDto(
    Guid ActivityId,
    string Title,
    ActivityType Type,
    bool IsMandatory,
    bool IsApproved);
