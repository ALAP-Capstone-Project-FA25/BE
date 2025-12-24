namespace ALAP.Entity.DTO.Request
{
    public class CreateUpdateUserTopicDto
    {
        public long CourseId { get; set; }
        public long TopicId { get; set; }
        public long? CurrentLessonId { get; set; }
        public bool? IsCompleted { get; set; }
    }
}


