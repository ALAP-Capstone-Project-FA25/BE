using ALAP.BLL.Interface;
using ALAP.DAL.DataBase;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace ALAP.BLL.Implement
{
    public class AdaptiveRecommendationBizLogic : AppBaseBizLogic, IAdaptiveRecommendationBizLogic
    {
        private readonly IUserWeakAreaRepository _weakAreaRepository;
        private readonly BaseDBContext _dbContext;

        public AdaptiveRecommendationBizLogic(
            BaseDBContext dbContext,
            IUserWeakAreaRepository weakAreaRepository) : base(dbContext)
        {
            _weakAreaRepository = weakAreaRepository;
            _dbContext = dbContext;
        }

        public async Task<List<AdaptiveCourseRecommendationDto>> GetAdaptiveCourseRecommendations(long userId)
        {
            // Get all weak areas for the user
            var weakAreas = await _weakAreaRepository.GetByUserId(userId);

            if (weakAreas.Count == 0)
            {
                return new List<AdaptiveCourseRecommendationDto>();
            }

            // Group weak areas by course
            var courseWeakAreas = weakAreas
                .GroupBy(wa => wa.CourseId)
                .ToList();

            // Get user's enrolled courses to check enrollment status
            var enrolledCourseIds = await _dbContext.UserCourses
                .Where(uc => uc.UserId == userId && uc.IsActive)
                .Select(uc => uc.CourseId)
                .ToListAsync();

            var recommendations = new List<AdaptiveCourseRecommendationDto>();

            foreach (var courseGroup in courseWeakAreas)
            {
                var courseId = courseGroup.Key;

                var course = await _dbContext.Courses
                    .Include(c => c.Category)
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null) continue;

                var weakLessons = courseGroup.ToList();
                var weakLessonCount = weakLessons.Count;
                var avgMasteryLevel = weakLessons.Average(wa => wa.MasteryLevel);

                // Calculate recommendation score
                // Higher score = more recommended
                int score = (int)(weakLessonCount * 10 + avgMasteryLevel * 2);

                // Get user's average performance to determine difficulty match
                var userAvgScore = await GetUserAverageScore(userId);
                int difficultyMatchBonus = 0;

                // If course difficulty matches user level, add bonus
                if (userAvgScore >= 70 && course.Difficulty >= 3)
                {
                    difficultyMatchBonus = 5;
                }
                else if (userAvgScore < 70 && course.Difficulty <= 2)
                {
                    difficultyMatchBonus = 5;
                }

                score += difficultyMatchBonus;

                // Build recommendation reason
                var reason = BuildRecommendationReason(weakLessonCount, avgMasteryLevel, course.Title);

                recommendations.Add(new AdaptiveCourseRecommendationDto
                {
                    CourseId = course.Id,
                    CourseTitle = course.Title,
                    CourseDescription = course.Description,
                    ImageUrl = course.ImageUrl,
                    RecommendationScore = score,
                    RecommendationReason = reason,
                    WeakLessonCount = weakLessonCount,
                    AverageMasteryLevel = Math.Round(avgMasteryLevel, 2),
                    IsEnrolled = enrolledCourseIds.Contains(courseId),
                    WeakAreas = weakLessons.Select(wa => new WeakAreaSummaryDto
                    {
                        LessonId = wa.LessonId,
                        LessonTitle = wa.Lesson?.Title ?? "Unknown",
                        ReferralCount = wa.ReferralCount,
                        MasteryLevel = wa.MasteryLevel
                    }).ToList()
                });
            }

            // Sort by recommendation score descending and return top 5
            return recommendations
                .OrderByDescending(r => r.RecommendationScore)
                .Take(5)
                .ToList();
        }

        public async Task<UserLearningStatisticsDto> GetUserLearningStatistics(long userId)
        {
            // Get all quiz attempts for the user
            var attempts = await _dbContext.UserTopicQuizAttempts
                .Include(a => a.Topic)
                    .ThenInclude(t => t.Course)
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.CompletedAt)
                .ToListAsync();

            if (attempts.Count == 0)
            {
                return new UserLearningStatisticsDto();
            }

            // Calculate basic statistics
            var totalAttempts = attempts.Count;
            var averageScore = attempts.Average(a => a.Score);
            var totalCorrect = attempts.Sum(a => a.CorrectAnswers);
            var totalWrong = attempts.Sum(a => a.WrongAnswers);
            var totalQuestions = attempts.Sum(a => a.TotalQuestions);
            var totalTimeSpent = attempts.Where(a => a.TimeSpent.HasValue).Sum(a => a.TimeSpent.Value);

            // Score distribution (0-20, 21-40, 41-60, 61-80, 81-100)
            var scoreDistribution = new List<ScoreDistributionDto>
            {
                new ScoreDistributionDto { Range = "0-20", Count = attempts.Count(a => a.Score >= 0 && a.Score <= 20) },
                new ScoreDistributionDto { Range = "21-40", Count = attempts.Count(a => a.Score >= 21 && a.Score <= 40) },
                new ScoreDistributionDto { Range = "41-60", Count = attempts.Count(a => a.Score >= 41 && a.Score <= 60) },
                new ScoreDistributionDto { Range = "61-80", Count = attempts.Count(a => a.Score >= 61 && a.Score <= 80) },
                new ScoreDistributionDto { Range = "81-100", Count = attempts.Count(a => a.Score >= 81 && a.Score <= 100) }
            };

            // Attempts by date (last 30 days)
            var thirtyDaysAgo = Utils.GetCurrentVNTime().AddDays(-30);
            var recentAttempts = attempts.Where(a => a.CompletedAt >= thirtyDaysAgo).ToList();

            var attemptsByDate = recentAttempts
                .GroupBy(a => a.CompletedAt.Date.ToString("yyyy-MM-dd"))
                .Select(g => new QuizAttemptByDateDto
                {
                    Date = g.Key,
                    AttemptCount = g.Count(),
                    AverageScore = g.Average(a => a.Score)
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Course performance
            var coursePerformance = attempts
                .Where(a => a.Topic?.Course != null)
                .GroupBy(a => new { a.Topic.CourseId, CourseTitle = a.Topic.Course.Title })
                .Select(g => new CoursePerformanceDto
                {
                    CourseId = g.Key.CourseId,
                    CourseTitle = g.Key.CourseTitle,
                    AttemptCount = g.Count(),
                    AverageScore = g.Average(a => a.Score),
                    TotalQuestions = g.Sum(a => a.TotalQuestions),
                    CorrectAnswers = g.Sum(a => a.CorrectAnswers)
                })
                .OrderByDescending(c => c.AttemptCount)
                .Take(10)
                .ToList();

            // Topic performance
            var topicPerformance = attempts
                .Where(a => a.Topic != null)
                .GroupBy(a => new { a.TopicId, TopicTitle = a.Topic.Title, CourseId = a.Topic.CourseId, CourseTitle = a.Topic.Course?.Title ?? "" })
                .Select(g => new TopicPerformanceDto
                {
                    TopicId = g.Key.TopicId,
                    TopicTitle = g.Key.TopicTitle,
                    CourseId = g.Key.CourseId,
                    CourseTitle = g.Key.CourseTitle,
                    AttemptCount = g.Count(),
                    AverageScore = g.Average(a => a.Score),
                    BestScore = g.Max(a => a.Score)
                })
                .OrderByDescending(t => t.AverageScore)
                .Take(10)
                .ToList();

            return new UserLearningStatisticsDto
            {
                TotalQuizAttempts = totalAttempts,
                AverageScore = Math.Round(averageScore, 2),
                TotalCorrectAnswers = totalCorrect,
                TotalWrongAnswers = totalWrong,
                TotalQuestionsAnswered = totalQuestions,
                TotalTimeSpent = totalTimeSpent,
                ScoreDistribution = scoreDistribution,
                AttemptsByDate = attemptsByDate,
                CoursePerformance = coursePerformance,
                TopicPerformance = topicPerformance
            };
        }

        private async Task<double> GetUserAverageScore(long userId)
        {
            var attempts = await _dbContext.UserTopicQuizAttempts
                .Where(a => a.UserId == userId)
                .ToListAsync();

            if (attempts.Count == 0) return 0;

            return attempts.Average(a => a.Score);
        }

        public async Task<List<AdaptiveLessonRecommendationDto>> GetAdaptiveLessonRecommendations(long userId)
        {
            // Get all weak areas for the user
            var weakAreas = await _weakAreaRepository.GetByUserId(userId);

            if (weakAreas.Count == 0)
            {
                return new List<AdaptiveLessonRecommendationDto>();
            }

            // Get user's enrolled courses to check enrollment status
            var enrolledCourseIds = await _dbContext.UserCourses
                .Where(uc => uc.UserId == userId && uc.IsActive)
                .Select(uc => uc.CourseId)
                .ToListAsync();

            var recommendations = new List<AdaptiveLessonRecommendationDto>();

            foreach (var weakArea in weakAreas.OrderByDescending(wa => wa.ReferralCount).Take(10))
            {
                var lesson = await _dbContext.Lessons
                    .Include(l => l.Topic)
                        .ThenInclude(t => t.Course)
                    .FirstOrDefaultAsync(l => l.Id == weakArea.LessonId);

                if (lesson == null) continue;

                // Get wrong questions for this lesson
                var wrongAnswers = await _dbContext.UserWrongAnswers
                    .Include(w => w.TopicQuestion)
                        .ThenInclude(q => q.Topic)
                            .ThenInclude(t => t.Course)
                    .Include(w => w.TopicQuestion)
                        .ThenInclude(q => q.TopicQuestionAnswers)
                    .Where(w => w.UserId == userId && w.ReferrerLessonId == lesson.Id)
                    .OrderByDescending(w => w.CreatedAt)
                    .ToListAsync();

                var wrongQuestionDetails = new List<WrongQuestionDetailDto>();
                foreach (var wrongAnswer in wrongAnswers)
                {
                    if (wrongAnswer.TopicQuestion != null)
                    {
                        var selectedIds = System.Text.Json.JsonSerializer.Deserialize<List<long>>(wrongAnswer.SelectedAnswerIds) ?? new List<long>();
                        var correctIds = System.Text.Json.JsonSerializer.Deserialize<List<long>>(wrongAnswer.CorrectAnswerIds) ?? new List<long>();

                        var selectedTexts = wrongAnswer.TopicQuestion.TopicQuestionAnswers
                            .Where(a => selectedIds.Contains(a.Id))
                            .Select(a => a.Answer)
                            .ToList();
                        var correctTexts = wrongAnswer.TopicQuestion.TopicQuestionAnswers
                            .Where(a => correctIds.Contains(a.Id))
                            .Select(a => a.Answer)
                            .ToList();

                        wrongQuestionDetails.Add(new WrongQuestionDetailDto
                        {
                            QuestionId = wrongAnswer.TopicQuestionId,
                            QuestionText = wrongAnswer.TopicQuestion.Question,
                            CourseTitle = wrongAnswer.TopicQuestion.Topic?.Course?.Title ?? lesson.Topic.Course.Title,
                            TopicTitle = wrongAnswer.TopicQuestion.Topic?.Title ?? lesson.Topic.Title,
                            SelectedAnswers = string.Join(", ", selectedTexts),
                            CorrectAnswers = string.Join(", ", correctTexts)
                        });
                    }
                }

                // Calculate recommendation score
                int score = weakArea.ReferralCount * 10 + (6 - weakArea.MasteryLevel) * 5;

                // Build recommendation reason
                string reason = $"Bạn đã trả lời sai {weakArea.ReferralCount} câu hỏi liên quan đến bài học này. Hãy xem lại video để củng cố kiến thức.";

                recommendations.Add(new AdaptiveLessonRecommendationDto
                {
                    LessonId = lesson.Id,
                    LessonTitle = lesson.Title,
                    LessonDescription = lesson.Description,
                    LessonVideoUrl = lesson.VideoUrl,
                    LessonType = (int)lesson.LessonType,
                    TopicId = lesson.TopicId,
                    TopicTitle = lesson.Topic.Title,
                    CourseId = lesson.Topic.CourseId,
                    CourseTitle = lesson.Topic.Course.Title,
                    CourseImageUrl = lesson.Topic.Course.ImageUrl,
                    RecommendationScore = score,
                    RecommendationReason = reason,
                    ReferralCount = weakArea.ReferralCount,
                    MasteryLevel = weakArea.MasteryLevel,
                    IsEnrolled = enrolledCourseIds.Contains(lesson.Topic.CourseId),
                    WrongQuestions = wrongQuestionDetails
                });
            }

            // Sort by recommendation score descending
            return recommendations
                .OrderByDescending(r => r.RecommendationScore)
                .ToList();
        }

        private string BuildRecommendationReason(int weakLessonCount, double avgMasteryLevel, string courseTitle)
        {
            if (weakLessonCount >= 5)
            {
                return $"Bạn đang gặp khó khăn với {weakLessonCount} bài học trong khóa học này. Khóa học này sẽ giúp bạn củng cố kiến thức.";
            }
            else if (weakLessonCount >= 3)
            {
                return $"Bạn cần cải thiện {weakLessonCount} phần kiến thức trong khóa học này.";
            }
            else
            {
                return $"Dựa trên kết quả quiz, khóa học này sẽ giúp bạn củng cố một số kiến thức còn yếu.";
            }
        }
    }
}

