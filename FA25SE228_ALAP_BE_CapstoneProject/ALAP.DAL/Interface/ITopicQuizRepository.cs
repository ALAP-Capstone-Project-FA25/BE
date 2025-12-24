using ALAP.Entity.Models;

namespace ALAP.DAL.Interface
{
    public interface ITopicQuizRepository
    {
        Task<UserTopicQuizAttempt> CreateAttempt(UserTopicQuizAttempt attempt);
        Task<UserTopicQuizAttempt?> GetLatestAttempt(long userId, long topicId);
        Task<List<UserTopicQuizAttempt>> GetAttemptsByUserTopic(long userTopicId);
        Task<UserWrongAnswer> CreateWrongAnswer(UserWrongAnswer wrongAnswer);
        Task<List<UserWrongAnswer>> GetWrongAnswersByAttempt(long attemptId);
        Task<List<UserWrongAnswer>> GetWrongAnswersByUserAndLesson(long userId, long lessonId);
    }
}

