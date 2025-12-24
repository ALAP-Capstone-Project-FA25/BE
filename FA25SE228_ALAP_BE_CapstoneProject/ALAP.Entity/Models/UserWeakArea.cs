using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("UserWeakAreas")]
    public class UserWeakArea : BaseEntity
    {
        [ForeignKey("User")]
        public long UserId { get; set; }

        [ForeignKey("Lesson")]
        public long LessonId { get; set; }

        [ForeignKey("Course")]
        public long CourseId { get; set; }

        public int ReferralCount { get; set; } = 1;
        public DateTime LastReferralAt { get; set; }
        public int MasteryLevel { get; set; } = 1; // 1-5, 1 is weakest

        [JsonIgnore]
        public virtual UserModel User { get; set; } = null;

        [JsonIgnore]
        public virtual LessonModel Lesson { get; set; } = null;

        [JsonIgnore]
        public virtual CourseModel Course { get; set; } = null;
    }
}

