using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("UserTopicQuizAttempts")]
    public class UserTopicQuizAttempt : BaseEntity
    {
        [ForeignKey("User")]
        public long UserId { get; set; }

        [ForeignKey("Topic")]
        public long TopicId { get; set; }

        [ForeignKey("UserTopic")]
        public long UserTopicId { get; set; }

        public int AttemptNumber { get; set; } = 1;
        public int Score { get; set; } // Percentage
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int? TimeSpent { get; set; } // Seconds
        public DateTime CompletedAt { get; set; }

        [JsonIgnore]
        public virtual UserModel User { get; set; } = null;

        [JsonIgnore]
        public virtual TopicModel Topic { get; set; } = null;

        [JsonIgnore]
        public virtual UserTopicModel UserTopic { get; set; } = null;
    }
}

