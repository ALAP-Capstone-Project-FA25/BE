using ALAP.Entity.Models;

namespace ALAP.Entity.DTO.Response
{
    public class UserTopicDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int OrderIndex { get; set; }

        public bool IsCurrent { get; set; }

        public long CourseId { get; set; }
        public long? UserTopicId { get; set; } // ID from UserTopics table

        public virtual ICollection<UserLessonDto> Lessons { get; set; } = new List<UserLessonDto>();

        public virtual ICollection<TopicQuestionModel> TopicQuestions { get; set; } = new List<TopicQuestionModel>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
