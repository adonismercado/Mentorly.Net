using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class AnalyticsRepository(MentorlyDbContext dbContext) : IAnalyticsRepository
{
    public async Task<AnalyticsOverviewData> GetOverviewAsync(CancellationToken c = default) => new(await dbContext.Courses.CountAsync(c), await dbContext.Enrollments.CountAsync(x => x.Status == EnrollmentStatus.Active, c), await dbContext.Enrollments.CountAsync(x => x.Status == EnrollmentStatus.Completed, c), await dbContext.Enrollments.CountAsync(x => x.Status == EnrollmentStatus.Expired, c), await dbContext.Submissions.CountAsync(x => x.Status == SubmissionStatus.Pending, c));
    public Task<bool> CourseExistsAsync(Guid courseId, CancellationToken c = default) => dbContext.Courses.AnyAsync(x => x.Id == courseId, c);
    public async Task<IReadOnlyList<DropOffData>> GetDropOffAsync(Guid courseId, CancellationToken c = default) => await (from unit in dbContext.Units join theme in dbContext.Themes on unit.Id equals theme.UnitId where unit.CourseId == courseId let enrolled = dbContext.Enrollments.Count(e => e.CourseId == courseId) let completed = dbContext.ThemeCompletions.Count(x => x.ThemeId == theme.Id && x.Enrollment.CourseId == courseId) orderby unit.OrderIndex, theme.OrderIndex select new DropOffData(unit.Id, unit.Title, theme.Id, theme.Title, enrolled, completed)).ToListAsync(c);
    public async Task<double?> GetCourseAverageCompletionDaysAsync(Guid courseId, CancellationToken c = default) { var values = await dbContext.Enrollments.Where(x => x.CourseId == courseId && x.Status == EnrollmentStatus.Completed && x.CompletedAt != null).Select(x => EF.Functions.DateDiffSecond(x.StartedAt, x.CompletedAt!.Value)).ToListAsync(c); return values.Count == 0 ? null : values.Average() / 86400d; }
    public async Task<IReadOnlyList<CompletionTimeData>> GetUnitCompletionTimesAsync(Guid courseId, CancellationToken c = default)
    {
        var rows = await (from unit in dbContext.Units
                          join theme in dbContext.Themes on unit.Id equals theme.UnitId
                          join completion in dbContext.ThemeCompletions on theme.Id equals completion.ThemeId
                          join enrollment in dbContext.Enrollments on completion.EnrollmentId equals enrollment.Id
                          where unit.CourseId == courseId
                          select new { UnitId = unit.Id, unit.Title, EnrollmentId = enrollment.Id, enrollment.StartedAt, completion.CompletedAt }).ToListAsync(c);

        var averages = rows.GroupBy(x => new { x.UnitId, x.Title })
            .Select(group => new CompletionTimeData(group.Key.UnitId, group.Key.Title, group.GroupBy(x => x.EnrollmentId).Average(enrollment => (enrollment.Max(x => x.CompletedAt) - enrollment.First().StartedAt).TotalDays)))
            .ToDictionary(x => x.UnitId);

        var units = await dbContext.Units.Where(x => x.CourseId == courseId).OrderBy(x => x.OrderIndex).Select(x => new { x.Id, x.Title }).ToListAsync(c);
        return units.Select(x => averages.TryGetValue(x.Id, out var value) ? value : new CompletionTimeData(x.Id, x.Title, null)).ToList();
    }
    public async Task<IReadOnlyList<PeerReviewBottleneckData>> GetPeerReviewBottlenecksAsync(Guid courseId, CancellationToken c = default) => await (from activity in dbContext.Activities join theme in dbContext.Themes on activity.ThemeId equals theme.Id join unit in dbContext.Units on theme.UnitId equals unit.Id where unit.CourseId == courseId && activity.ApprovalStrategy == ApprovalStrategy.PeerReview let pending = dbContext.Submissions.Count(s => s.ActivityId == activity.Id && s.Status == SubmissionStatus.Pending) let escalated = dbContext.Submissions.Count(s => s.ActivityId == activity.Id && s.Status == SubmissionStatus.Escalated) let oldest = dbContext.Submissions.Where(s => s.ActivityId == activity.Id && s.Status == SubmissionStatus.Pending).Select(s => (DateTime?)s.SubmittedAt).Min() select new PeerReviewBottleneckData(activity.Id, activity.Title, pending, escalated, oldest)).ToListAsync(c);
    public async Task<IReadOnlyList<EnrollmentHistoryData>> GetEnrollmentHistoryAsync(Guid courseId, CancellationToken c = default) => await dbContext.Enrollments.Where(x => x.CourseId == courseId).OrderBy(x => x.StudentId).ThenBy(x => x.AttemptNumber).Select(x => new EnrollmentHistoryData(x.Id, x.StudentId, x.AttemptNumber, x.Status, x.StartedAt, x.ExpiresAt, x.CompletedAt)).ToListAsync(c);
}
