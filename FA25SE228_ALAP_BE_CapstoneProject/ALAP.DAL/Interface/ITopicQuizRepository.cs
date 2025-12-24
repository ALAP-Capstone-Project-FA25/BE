using App.Entity.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.DAL.Interface
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

