using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("UserTopics")]
    public class UserTopicModel : BaseEntity
    {
        [ForeignKey("UserCourse")]
        public long UserCourseId { get; set; }

        [ForeignKey("Topic")]
        public long TopicId { get; set; }

        public int Progress { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
        public int Idx { get; set; }

        [JsonIgnore]
        public virtual UserCourseModel UserCourse { get; set; } = null;

        [JsonIgnore]
        public virtual TopicModel Topic { get; set; } = null;

        public virtual ICollection<UserLessonModel> UserLessons { get; set; } = new List<UserLessonModel>();
    }
}


