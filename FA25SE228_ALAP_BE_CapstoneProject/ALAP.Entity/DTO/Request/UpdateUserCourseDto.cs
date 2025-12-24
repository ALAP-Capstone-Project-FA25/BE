namespace ALAP.Entity.DTO.Request
{
    public class UpdateUserCourseDto
    {
        public long CourseId { get; set; }
        public long? CurrentTopicId { get; set; }
        public long? CurrentLessonId { get; set; }
        public bool? IsCompleted { get; set; }
    }
}


