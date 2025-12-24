using ALAP.Entity.Models;

namespace ALAP.Entity.DTO.Response
{
    public class UserCourseResponseDto
    {
        public long UserId { get; set; }

        public long CourseId { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDone { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public long? PaymentId { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }

        public virtual UserDTO User { get; set; } = null;

        public virtual ICollection<UserTopicModel> UserTopics { get; set; } = new List<UserTopicModel>();
    }
}
