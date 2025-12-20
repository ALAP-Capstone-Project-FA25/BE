using ALAP.BLL.Interface;
using ALAP.DAL.DataBase;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Enums;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class TopicQuizBizLogic : AppBaseBizLogic, ITopicQuizBizLogic
    {
        private readonly ITopicQuizRepository _quizRepository;
        private readonly IUserWeakAreaRepository _weakAreaRepository;
        private readonly BaseDBContext _dbContext;

        public TopicQuizBizLogic(
            BaseDBContext dbContext,
            ITopicQuizRepository quizRepository,
            IUserWeakAreaRepository weakAreaRepository) : base(dbContext)
        {
            _quizRepository = quizRepository;
            _weakAreaRepository = weakAreaRepository;
            _dbContext = dbContext;
        }

        public async Task<QuizResultDto> SubmitTopicQuiz(long userId, SubmitTopicQuizDto dto)
        {
            // Resolve UserTopicId if not provided or invalid
            long userTopicId = dto.UserTopicId;
            if (userTopicId <= 0)
            {
                // Get topic to find courseId
                var topic = await _dbContext.Topics
                    .FirstOrDefaultAsync(t => t.Id == dto.TopicId);

                if (topic == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy topic này.");
                }

                // Get UserCourse
                var userCourse = await _dbContext.UserCourses
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == topic.CourseId);

                if (userCourse == null)
                {
                    throw new KeyNotFoundException("Bạn chưa đăng ký khóa học này.");
                }

                // Get UserTopic, create if not exists
                var userTopic = await _dbContext.UserTopics
                    .FirstOrDefaultAsync(ut => ut.UserCourseId == userCourse.Id && ut.TopicId == dto.TopicId);

                if (userTopic == null)
                {
                    // Get topic to get OrderIndex
                    var topicForUserTopic = await _dbContext.Topics
                        .FirstOrDefaultAsync(t => t.Id == dto.TopicId);

                    if (topicForUserTopic == null)
                    {
                        throw new KeyNotFoundException("Không tìm thấy topic này.");
                    }

                    // Get all existing UserTopics for this course to determine if this should be locked
                    var existingUserTopics = await _dbContext.UserTopics
                        .Where(ut => ut.UserCourseId == userCourse.Id)
                        .OrderBy(ut => ut.Idx)
                        .ToListAsync();

                    // Determine if topic should be locked (if there are previous topics that are not completed)
                    bool shouldLock = existingUserTopics.Any(ut => ut.Idx < topicForUserTopic.OrderIndex);

                    // Create new UserTopic
                    userTopic = new UserTopicModel
                    {
                        UserCourseId = userCourse.Id,
                        TopicId = dto.TopicId,
                        Idx = topicForUserTopic.OrderIndex,
                        IsLocked = shouldLock,
                        Progress = 0
                    };

                    await _dbContext.UserTopics.AddAsync(userTopic);
                    await _dbContext.SaveChangesAsync();
                }

                userTopicId = userTopic.Id;
            }

            // Get all questions for the topic
            var questions = await _dbContext.TopicQuestions
                .Include(q => q.TopicQuestionAnswers)
                .Where(q => q.TopicId == dto.TopicId)
                .ToListAsync();

            if (questions.Count == 0)
            {
                throw new KeyNotFoundException("Không tìm thấy câu hỏi cho topic này.");
            }

            // Calculate score
            int correctCount = 0;
            int wrongCount = 0;
            var wrongAnswers = new List<(TopicQuestionModel question, List<long> selected, List<long> correct)>();

            foreach (var question in questions)
            {
                var userSelected = dto.Answers.ContainsKey(question.Id) ? dto.Answers[question.Id] : new List<long>();
                var correctAnswerIds = question.TopicQuestionAnswers
                    .Where(a => a.IsCorrect)
                    .Select(a => a.Id)
                    .OrderBy(id => id)
                    .ToList();

                var userSelectedSorted = userSelected.OrderBy(id => id).ToList();

                bool isCorrect = userSelectedSorted.Count == correctAnswerIds.Count &&
                                userSelectedSorted.SequenceEqual(correctAnswerIds);

                if (isCorrect)
                {
                    correctCount++;
                }
                else
                {
                    wrongCount++;
                    wrongAnswers.Add((question, userSelected, correctAnswerIds));
                }
            }

            int score = questions.Count > 0 ? (int)Math.Round((double)correctCount / questions.Count * 100) : 0;

            // Get latest attempt number
            var latestAttempt = await _quizRepository.GetLatestAttempt(userId, dto.TopicId);
            int attemptNumber = latestAttempt != null ? latestAttempt.AttemptNumber + 1 : 1;

            // Create quiz attempt
            var attempt = new UserTopicQuizAttempt
            {
                UserId = userId,
                TopicId = dto.TopicId,
                UserTopicId = userTopicId,
                AttemptNumber = attemptNumber,
                Score = score,
                TotalQuestions = questions.Count,
                CorrectAnswers = correctCount,
                WrongAnswers = wrongCount,
                TimeSpent = dto.TimeSpent,
                CompletedAt = Utils.GetCurrentVNTime()
            };

            var savedAttempt = await _quizRepository.CreateAttempt(attempt);

            // Save wrong answers and update weak areas
            var suggestedLessons = new Dictionary<long, SuggestedLessonDto>();

            // Get current topic and course info for wrong questions
            var currentTopic = await _dbContext.Topics
                .Include(t => t.Course)
                .FirstOrDefaultAsync(t => t.Id == dto.TopicId);

            foreach (var (question, selected, correct) in wrongAnswers)
            {
                if (question.ReferrerLessonId.HasValue)
                {
                    var lesson = await _dbContext.Lessons
                        .Include(l => l.Topic)
                            .ThenInclude(t => t.Course)
                        .FirstOrDefaultAsync(l => l.Id == question.ReferrerLessonId.Value);

                    if (lesson != null)
                    {
                        // Get answer texts for display
                        var selectedAnswerTexts = question.TopicQuestionAnswers
                            .Where(a => selected.Contains(a.Id))
                            .Select(a => a.Answer)
                            .ToList();
                        var correctAnswerTexts = question.TopicQuestionAnswers
                            .Where(a => correct.Contains(a.Id))
                            .Select(a => a.Answer)
                            .ToList();

                        // Save wrong answer
                        var wrongAnswer = new UserWrongAnswer
                        {
                            UserId = userId,
                            TopicQuestionId = question.Id,
                            ReferrerLessonId = lesson.Id,
                            QuizAttemptId = savedAttempt.Id,
                            SelectedAnswerIds = JsonSerializer.Serialize(selected),
                            CorrectAnswerIds = JsonSerializer.Serialize(correct)
                        };
                        await _quizRepository.CreateWrongAnswer(wrongAnswer);

                        // Update or create weak area
                        var weakArea = await _weakAreaRepository.GetByUserAndLesson(userId, lesson.Id);

                        // Calculate mastery level (1-5, lower is weaker)
                        // Get total attempts for this topic to calculate mastery
                        var totalAttempts = await _quizRepository.GetAttemptsByUserTopic(userTopicId);
                        var masteryLevel = Math.Max(1, Math.Min(5, 5 - (weakArea?.ReferralCount ?? 0) / Math.Max(1, totalAttempts.Count) * 5));

                        var newWeakArea = new UserWeakArea
                        {
                            UserId = userId,
                            LessonId = lesson.Id,
                            CourseId = lesson.Topic.CourseId,
                            ReferralCount = weakArea?.ReferralCount + 1 ?? 1,
                            LastReferralAt = Utils.GetCurrentVNTime(),
                            MasteryLevel = masteryLevel
                        };

                        var isNewWeakArea = weakArea == null;
                        await _weakAreaRepository.CreateOrUpdate(newWeakArea);

                        // Create notification when new weak area is detected (knowledge reinforcement needed)
                        if (isNewWeakArea)
                        {
                            try
                            {
                                var notification = new NotificationModel
                                {
                                    UserId = userId,
                                    Type = NotificationType.KNOWLEDGE_REINFORCEMENT,
                                    Title = "Có kiến thức cần củng cố mới",
                                    Message = $"Bạn có bài học mới cần củng cố: {lesson.Title}. Hãy click vào góc dưới bên phải để xem chi tiết.",
                                    LinkUrl = "/profile",
                                    IsRead = false,
                                    CreatedAt = Utils.GetCurrentVNTime(),
                                    UpdatedAt = Utils.GetCurrentVNTime()
                                };

                                await _dbContext.Notifications.AddAsync(notification);
                                await _dbContext.SaveChangesAsync();
                            }
                            catch
                            {
                                // Silently fail - notification is not critical
                            }
                        }

                        // Add to suggested lessons
                        if (!suggestedLessons.ContainsKey(lesson.Id))
                        {
                            suggestedLessons[lesson.Id] = new SuggestedLessonDto
                            {
                                LessonId = lesson.Id,
                                LessonTitle = lesson.Title,
                                LessonDescription = lesson.Description,
                                LessonVideoUrl = lesson.VideoUrl,
                                TopicId = lesson.TopicId,
                                TopicTitle = lesson.Topic.Title,
                                CourseId = lesson.Topic.CourseId,
                                CourseTitle = lesson.Topic.Course.Title,
                                WrongQuestionCount = 1,
                                WrongQuestions = new List<WrongQuestionInfoDto>()
                            };
                        }
                        else
                        {
                            suggestedLessons[lesson.Id].WrongQuestionCount++;
                        }

                        // Add wrong question info
                        suggestedLessons[lesson.Id].WrongQuestions.Add(new WrongQuestionInfoDto
                        {
                            QuestionId = question.Id,
                            QuestionText = question.Question,
                            CourseTitle = currentTopic?.Course?.Title ?? lesson.Topic.Course.Title,
                            TopicTitle = currentTopic?.Title ?? lesson.Topic.Title,
                            SelectedAnswers = string.Join(", ", selectedAnswerTexts),
                            CorrectAnswers = string.Join(", ", correctAnswerTexts)
                        });
                    }
                }
            }

            return new QuizResultDto
            {
                AttemptId = savedAttempt.Id,
                Score = score,
                TotalQuestions = questions.Count,
                CorrectAnswers = correctCount,
                WrongAnswers = wrongCount,
                AttemptNumber = attemptNumber,
                SuggestedLessons = suggestedLessons.Values.ToList()
            };
        }

        public async Task<List<SuggestedLessonDto>> GetSuggestedLessons(long userId, long topicId)
        {
            // Get topic to find courseId
            var topic = await _dbContext.Topics
                .FirstOrDefaultAsync(t => t.Id == topicId);

            if (topic == null)
            {
                return new List<SuggestedLessonDto>();
            }

            // Get UserCourse
            var userCourse = await _dbContext.UserCourses
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == topic.CourseId);

            if (userCourse == null)
            {
                return new List<SuggestedLessonDto>();
            }

            // Get UserTopicId
            var userTopic = await _dbContext.UserTopics
                .Where(ut => ut.UserCourseId == userCourse.Id && ut.TopicId == topicId)
                .FirstOrDefaultAsync();

            if (userTopic == null)
            {
                return new List<SuggestedLessonDto>();
            }

            // Get all wrong answers for this topic
            var attempts = await _quizRepository.GetAttemptsByUserTopic(userTopic.Id);

            var suggestedLessons = new Dictionary<long, SuggestedLessonDto>();

            foreach (var attempt in attempts)
            {
                var wrongAnswers = await _quizRepository.GetWrongAnswersByAttempt(attempt.Id);

                foreach (var wrongAnswer in wrongAnswers)
                {
                    if (wrongAnswer.ReferrerLesson != null)
                    {
                        var lesson = wrongAnswer.ReferrerLesson;
                        var lessonTopic = await _dbContext.Topics
                            .Include(t => t.Course)
                            .FirstOrDefaultAsync(t => t.Id == lesson.TopicId);

                        if (lessonTopic != null && !suggestedLessons.ContainsKey(lesson.Id))
                        {
                            suggestedLessons[lesson.Id] = new SuggestedLessonDto
                            {
                                LessonId = lesson.Id,
                                LessonTitle = lesson.Title,
                                LessonDescription = lesson.Description,
                                TopicId = lesson.TopicId,
                                TopicTitle = lessonTopic.Title,
                                CourseId = lessonTopic.CourseId,
                                CourseTitle = lessonTopic.Course.Title,
                                WrongQuestionCount = wrongAnswers.Count(wa => wa.ReferrerLessonId == lesson.Id)
                            };
                        }
                    }
                }
            }

            return suggestedLessons.Values.ToList();
        }
    }
}

