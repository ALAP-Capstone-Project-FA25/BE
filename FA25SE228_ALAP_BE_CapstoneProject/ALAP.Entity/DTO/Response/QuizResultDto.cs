namespace ALAP.Entity.DTO.Response
{
    public class QuizResultDto
    {
        public long AttemptId { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int AttemptNumber { get; set; }
        public List<SuggestedLessonDto> SuggestedLessons { get; set; } = new List<SuggestedLessonDto>();
    }

    public class SuggestedLessonDto
    {
        public long LessonId { get; set; }
        public string LessonTitle { get; set; }
        public string LessonDescription { get; set; }
        public string LessonVideoUrl { get; set; }
        public long TopicId { get; set; }
        public string TopicTitle { get; set; }
        public long CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int WrongQuestionCount { get; set; } // Số câu hỏi sai liên quan đến lesson này
        public List<WrongQuestionInfoDto> WrongQuestions { get; set; } = new List<WrongQuestionInfoDto>(); // Chi tiết các câu hỏi sai
    }

    public class WrongQuestionInfoDto
    {
        public long QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string CourseTitle { get; set; }
        public string TopicTitle { get; set; }
        public string SelectedAnswers { get; set; } // Đáp án user chọn
        public string CorrectAnswers { get; set; } // Đáp án đúng
    }
}

