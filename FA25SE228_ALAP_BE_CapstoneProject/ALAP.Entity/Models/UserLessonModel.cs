using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("UserLessons")]
    public class UserLessonModel : BaseEntity
    {
        [ForeignKey("UserTopic")] 
        public long UserTopicId { get; set; }

        [ForeignKey("Lesson")] 
        public long LessonId { get; set; }

        public bool IsDone { get; set; } = false;

        [JsonIgnore]
        public virtual UserTopicModel UserTopic { get; set; } = null;

        [JsonIgnore]
        public virtual LessonModel Lesson { get; set; } = null;
    }
}


