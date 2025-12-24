namespace App.Entity.DTO.Request
{
    public class CreateUpdateUserLessonDto
    {
        public long CourseId { get; set; }
        public long TopicId { get; set; }
        public long LessonId { get; set; }
        public bool IsCompleted { get; set; } = true;
    }
}


