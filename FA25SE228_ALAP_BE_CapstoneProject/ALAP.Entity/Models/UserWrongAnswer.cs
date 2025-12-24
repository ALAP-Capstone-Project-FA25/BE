using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("UserWrongAnswers")]
    public class UserWrongAnswer : BaseEntity
    {
        [ForeignKey("User")]
        public long UserId { get; set; }

        [ForeignKey("TopicQuestion")]
        public long TopicQuestionId { get; set; }

        [ForeignKey("ReferrerLesson")]
        public long ReferrerLessonId { get; set; }

        [ForeignKey("QuizAttempt")]
        public long QuizAttemptId { get; set; }

        public string SelectedAnswerIds { get; set; } = string.Empty; // JSON array
        public string CorrectAnswerIds { get; set; } = string.Empty; // JSON array

        [JsonIgnore]
        public virtual UserModel User { get; set; } = null;

        [JsonIgnore]
        public virtual TopicQuestionModel TopicQuestion { get; set; } = null;

        [JsonIgnore]
        public virtual LessonModel ReferrerLesson { get; set; } = null;

        [JsonIgnore]
        public virtual UserTopicQuizAttempt QuizAttempt { get; set; } = null;
    }
}

