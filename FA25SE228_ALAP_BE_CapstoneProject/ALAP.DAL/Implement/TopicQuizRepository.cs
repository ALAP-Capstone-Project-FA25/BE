using ALAP.DAL;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace ALAP.DAL.Implement
{
    public class TopicQuizRepository : AppBaseRepository, ITopicQuizRepository
    {
        private readonly BaseDBContext _dbContext;

        public TopicQuizRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserTopicQuizAttempt> CreateAttempt(UserTopicQuizAttempt attempt)
        {
            await _dbContext.UserTopicQuizAttempts.AddAsync(attempt);
            await _dbContext.SaveChangesAsync();
            return attempt;
        }

        public async Task<UserTopicQuizAttempt?> GetLatestAttempt(long userId, long topicId)
        {
            return await _dbContext.UserTopicQuizAttempts
                .Where(a => a.UserId == userId && a.TopicId == topicId)
                .OrderByDescending(a => a.CompletedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserTopicQuizAttempt>> GetAttemptsByUserTopic(long userTopicId)
        {
            return await _dbContext.UserTopicQuizAttempts
                .Where(a => a.UserTopicId == userTopicId)
                .OrderByDescending(a => a.CompletedAt)
                .ToListAsync();
        }

        public async Task<UserWrongAnswer> CreateWrongAnswer(UserWrongAnswer wrongAnswer)
        {
            await _dbContext.UserWrongAnswers.AddAsync(wrongAnswer);
            await _dbContext.SaveChangesAsync();
            return wrongAnswer;
        }

        public async Task<List<UserWrongAnswer>> GetWrongAnswersByAttempt(long attemptId)
        {
            return await _dbContext.UserWrongAnswers
                .Include(w => w.ReferrerLesson)
                .Include(w => w.TopicQuestion)
                .Where(w => w.QuizAttemptId == attemptId)
                .ToListAsync();
        }

        public async Task<List<UserWrongAnswer>> GetWrongAnswersByUserAndLesson(long userId, long lessonId)
        {
            return await _dbContext.UserWrongAnswers
                .Include(w => w.TopicQuestion)
                    .ThenInclude(q => q.Topic)
                        .ThenInclude(t => t.Course)
                .Include(w => w.TopicQuestion)
                    .ThenInclude(q => q.TopicQuestionAnswers)
                .Where(w => w.UserId == userId && w.ReferrerLessonId == lessonId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }
    }
}

