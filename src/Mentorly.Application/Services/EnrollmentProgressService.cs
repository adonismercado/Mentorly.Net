using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Application.Services;

public sealed class EnrollmentProgressService(
    IEnrollmentRepository enrollmentRepository,
    ICourseRepository courseRepository,
    IThemeCompletionRepository themeCompletionRepository,
    IEnrollmentProgressRepository progressRepository,
    ISubmissionRepository submissionRepository,
    IQuizRepository quizRepository,
    IUnitRepository unitRepository,
    IThemeRepository themeRepository,
    IActivityRepository activityRepository,
    ICertificateService certificateService,
    IGamificationService gamificationService,
    IUnitOfWork unitOfWork,
    IStudentRepository? studentRepository = null) : IEnrollmentProgressService, ICourseCompletionService
{
    public async Task<IReadOnlyList<EnrollmentDto>> GetStudentEnrollmentsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var enrollments = await enrollmentRepository.GetByStudentIdAsync(studentId, cancellationToken);
        return enrollments.Select(enrollment => MapEnrollment(enrollment)).ToList();
    }

    public async Task<IReadOnlyList<EnrollmentDto>> GetStudentEnrollmentsAsAdminAsync(Guid adminId, Guid studentId, CancellationToken cancellationToken = default)
    {
        await EnsureAdminAsync(adminId, cancellationToken);
        return await GetStudentEnrollmentsAsync(studentId, cancellationToken);
    }

    public async Task<EnrollmentDto?> RestartAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null)
        {
            return null;
        }

        var latest = await enrollmentRepository.GetLatestByStudentAndCourseAsync(studentId, courseId, cancellationToken);
        if (latest is null)
        {
            throw new InvalidOperationException("An expired enrollment is required before restarting a course.");
        }

        latest.RefreshStatus(DateTime.UtcNow);
        if (latest.Status != EnrollmentStatus.Expired)
        {
            throw new InvalidOperationException("Only expired enrollments can be restarted.");
        }

        var attempt = await enrollmentRepository.GetNextAttemptNumberAsync(studentId, courseId, cancellationToken);
        var enrollment = Enrollment.CreateNew(studentId, courseId, attempt, DateTime.UtcNow);
        enrollmentRepository.Add(enrollment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return MapEnrollment(enrollment, course.Title);
    }

    public async Task<EnrollmentProgressDto?> GetProgressAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);
        return enrollment is null ? null : await BuildProgressAsync(enrollment, cancellationToken);
    }

    public async Task<EnrollmentProgressDto?> GetProgressAsAdminAsync(Guid adminId, Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        await EnsureAdminAsync(adminId, cancellationToken);
        return await GetProgressAsync(enrollmentId, cancellationToken);
    }

    public async Task<EnrollmentProgressDto?> CompleteThemeAsync(Guid enrollmentId, Guid themeId, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);
        if (enrollment is null)
        {
            return null;
        }

        enrollment.RefreshStatus(DateTime.UtcNow);
        if (enrollment.Status != EnrollmentStatus.Active)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException("Themes cannot be completed on an expired or completed enrollment.");
        }

        if (!await progressRepository.ThemeBelongsToCourseAsync(themeId, enrollment.CourseId, cancellationToken))
        {
            throw new InvalidOperationException("The theme does not belong to this enrollment course.");
        }

        if (!await themeCompletionRepository.ExistsAsync(enrollmentId, themeId, cancellationToken))
        {
            themeCompletionRepository.Add(new ThemeCompletion(enrollmentId, themeId, DateTime.UtcNow));
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await gamificationService.AwardAsync(enrollment.StudentId, Domain.Enums.GamificationEventType.ThemeCompleted, themeId, cancellationToken);
        }

        return await EvaluateEnrollmentAsync(enrollment, cancellationToken);
    }

    public async Task<EnrollmentStatusDto?> GetStatusAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);
        if (enrollment is null)
        {
            return null;
        }

        enrollment.RefreshStatus(DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new EnrollmentStatusDto(enrollment.Id, enrollment.Status, enrollment.StartedAt, enrollment.ExpiresAt, enrollment.Status == EnrollmentStatus.Active);
    }

    public async Task<CertificateDto?> GetCertificateAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);
        if (enrollment is null)
        {
            return null;
        }

        await EvaluateEnrollmentAsync(enrollment, cancellationToken);
        return enrollment.Status == EnrollmentStatus.Completed && enrollment.CertificateUrl is not null
            ? new CertificateDto(enrollment.Id, enrollment.CertificateUrl, enrollment.CompletedAt!.Value)
            : null;
    }

    public async Task<EnrollmentProgressDto?> EvaluateAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);
        return enrollment is null ? null : await EvaluateEnrollmentAsync(enrollment, cancellationToken);
    }

    private async Task<Enrollment?> GetOwnedEnrollmentAsync(Guid enrollmentId, Guid studentId, CancellationToken cancellationToken)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);
        return enrollment is not null && enrollment.StudentId == studentId ? enrollment : null;
    }

    private async Task EnsureAdminAsync(Guid adminId, CancellationToken cancellationToken)
    {
        var admin = studentRepository is null ? null : await studentRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin?.Role != StudentRole.Admin)
        {
            throw new InvalidOperationException("Only an administrator can view student progress.");
        }
    }

    private async Task<EnrollmentProgressDto> EvaluateEnrollmentAsync(Enrollment enrollment, CancellationToken cancellationToken)
    {
        enrollment.RefreshStatus(DateTime.UtcNow);
        var progress = await BuildProgressAsync(enrollment, cancellationToken, saveStatus: false);
        if (enrollment.Status == EnrollmentStatus.Active && progress.IsCompleted)
        {
            enrollment.Complete(certificateService.CreateCertificateUrl(enrollment.Id), DateTime.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            progress = progress with { Status = enrollment.Status, IsCompleted = true, CertificateUrl = enrollment.CertificateUrl };
        }
        else
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return progress;
    }

    private async Task<EnrollmentProgressDto> BuildProgressAsync(Enrollment enrollment, CancellationToken cancellationToken, bool saveStatus = true)
    {
        var themeIds = await progressRepository.GetThemeIdsAsync(enrollment.CourseId, cancellationToken);
        var completedThemeIds = (await themeCompletionRepository.GetByEnrollmentIdAsync(enrollment.Id, cancellationToken)).Select(x => x.ThemeId).ToHashSet();
        var requiredActivityIds = await progressRepository.GetMandatoryActivityIdsAsync(enrollment.CourseId, cancellationToken);
        var approvedActivityIds = (await submissionRepository.GetApprovedActivityIdsAsync(enrollment.Id, requiredActivityIds, cancellationToken)).ToHashSet();
        approvedActivityIds.UnionWith(await quizRepository.GetPassedActivityIdsAsync(enrollment.Id, requiredActivityIds, cancellationToken));
        var completedThemes = themeIds.Count(x => completedThemeIds.Contains(x));
        var approvedActivities = requiredActivityIds.Count(approvedActivityIds.Contains);
        var total = themeIds.Count + requiredActivityIds.Count;
        var completed = completedThemes + approvedActivities;
        var isCompleted = total > 0 && completed == total && enrollment.Status == EnrollmentStatus.Active;
        var percentage = total == 0 ? 0 : (int)Math.Round(completed * 100m / total, MidpointRounding.AwayFromZero);
        var units = await unitRepository.GetByCourseIdAsync(enrollment.CourseId, cancellationToken);
        var unitProgress = new List<EnrollmentUnitProgressDto>();

        foreach (var unit in units)
        {
            var themes = await themeRepository.GetByUnitIdAsync(unit.Id, cancellationToken);
            var themeProgress = new List<EnrollmentThemeProgressDto>();
            foreach (var theme in themes)
            {
                var themeActivities = await activityRepository.GetByThemeIdAsync(theme.Id, cancellationToken);
                themeProgress.Add(new EnrollmentThemeProgressDto(
                    theme.Id,
                    theme.Title,
                    theme.ContentText,
                    theme.OrderIndex,
                    completedThemeIds.Contains(theme.Id),
                    themeActivities.Select(activity => new EnrollmentActivityProgressDto(
                        activity.Id,
                        activity.Title,
                        activity.Type,
                        activity.IsMandatory,
                        !activity.IsMandatory || approvedActivityIds.Contains(activity.Id))).ToArray()));
            }

            var unitActivities = themeProgress.SelectMany(theme => theme.Activities).ToArray();
            var mandatoryActivities = unitActivities.Where(activity => activity.IsMandatory).ToArray();
            unitProgress.Add(new EnrollmentUnitProgressDto(
                unit.Id,
                unit.Title,
                themes.Count(theme => completedThemeIds.Contains(theme.Id)),
                themes.Count,
                mandatoryActivities.Count(activity => approvedActivityIds.Contains(activity.ActivityId)),
                mandatoryActivities.Length,
                themeProgress.ToArray()));
        }

        var blockedUnit = unitProgress.FirstOrDefault(unit =>
            unit.ApprovedMandatoryActivities < unit.TotalMandatoryActivities);
        var canSubmitNextUnit = blockedUnit is null;
        var blockedReason = canSubmitNextUnit
            ? null
            : "Debes aprobar el ejercicio obligatorio de la unidad anterior.";

        if (saveStatus)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new EnrollmentProgressDto(
            enrollment.Id,
            enrollment.Status,
            enrollment.StartedAt,
            enrollment.ExpiresAt,
            themeIds.Count,
            completedThemes,
            requiredActivityIds.Count,
            approvedActivities,
            percentage,
            isCompleted,
            enrollment.CertificateUrl,
            canSubmitNextUnit,
            blockedReason,
            unitProgress.ToArray());
    }

    private static EnrollmentDto MapEnrollment(Enrollment enrollment, string? courseTitle = null) => new(
        enrollment.Id,
        enrollment.StudentId,
        enrollment.CourseId,
        courseTitle ?? enrollment.Course?.Title ?? string.Empty,
        enrollment.AttemptNumber,
        enrollment.StartedAt,
        enrollment.ExpiresAt,
        enrollment.Status,
        enrollment.CertificateUrl);
}
