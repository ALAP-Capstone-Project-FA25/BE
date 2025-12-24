using ALAP.Entity.Models;

namespace ALAP.Entity.DTO.Request
{
    public class UserCourseDto
    {
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
        public virtual UserModel User { get; set; } 
    }
}
