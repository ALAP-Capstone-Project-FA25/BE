namespace ALAP.Entity.DTO.Response
{
    public class AdaptiveLessonRecommendationDto
    {
        public long LessonId { get; set; }
        public string LessonTitle { get; set; }
        public string LessonDescription { get; set; }
        public string LessonVideoUrl { get; set; }
        public int LessonType { get; set; } // 1 = VIDEO, 2 = DOCUMENT
        public long TopicId { get; set; }
        public string TopicTitle { get; set; }
        public long CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseImageUrl { get; set; }
        public int RecommendationScore { get; set; }
        public string RecommendationReason { get; set; }
        public int ReferralCount { get; set; }
        public int MasteryLevel { get; set; }
        public bool IsEnrolled { get; set; }
        public List<WrongQuestionDetailDto> WrongQuestions { get; set; } = new List<WrongQuestionDetailDto>();
    }

    public class WrongQuestionDetailDto
    {
        public long QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string CourseTitle { get; set; }
        public string TopicTitle { get; set; }
        public string SelectedAnswers { get; set; }
        public string CorrectAnswers { get; set; }
    }
}

