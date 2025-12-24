using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("UserCourses")]
    public class UserCourseModel : BaseEntity
    {
        [ForeignKey("User")] 
        public long UserId { get; set; }

        [ForeignKey("Course")] 
        public long CourseId { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDone { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public long? PaymentId { get; set; }

        public string? Title { get; set; }

        public long CurrentTopicId { get; set; } = 0;
        public long CurrentLessonId { get; set; } = 0;
        public double ProgressPercent { get; set; }
        public string? Description { get; set; }

        [JsonIgnore]
        public virtual UserModel User { get; set; } = null;

        [JsonIgnore]
        public virtual CourseModel Course { get; set; } = null;


    }
}


