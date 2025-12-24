using System;
using System.Collections.Generic;

namespace ALAP.Entity.DTO.Response
{
    public class UserLearningStatisticsDto
    {
        public int TotalQuizAttempts { get; set; }
        public double AverageScore { get; set; }
        public int TotalCorrectAnswers { get; set; }
        public int TotalWrongAnswers { get; set; }
        public int TotalQuestionsAnswered { get; set; }
        public int TotalTimeSpent { get; set; } // In seconds
        public List<ScoreDistributionDto> ScoreDistribution { get; set; } = new List<ScoreDistributionDto>();
        public List<QuizAttemptByDateDto> AttemptsByDate { get; set; } = new List<QuizAttemptByDateDto>();
        public List<CoursePerformanceDto> CoursePerformance { get; set; } = new List<CoursePerformanceDto>();
        public List<TopicPerformanceDto> TopicPerformance { get; set; } = new List<TopicPerformanceDto>();
    }

    public class ScoreDistributionDto
    {
        public string Range { get; set; } = string.Empty; // e.g., "0-20", "21-40", etc.
        public int Count { get; set; }
    }

    public class QuizAttemptByDateDto
    {
        public string Date { get; set; } = string.Empty; // Format: "YYYY-MM-DD"
        public int AttemptCount { get; set; }
        public double AverageScore { get; set; }
    }

    public class CoursePerformanceDto
    {
        public long CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public int AttemptCount { get; set; }
        public double AverageScore { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
    }

    public class TopicPerformanceDto
    {
        public long TopicId { get; set; }
        public string TopicTitle { get; set; } = string.Empty;
        public long CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public int AttemptCount { get; set; }
        public double AverageScore { get; set; }
        public int BestScore { get; set; }
    }
}

