namespace ALAP.Entity.DTO.Request
{
    public class CreateUpdateLessonNoteDto
    {
        public long Id { get; set; }
        public long LessonId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Time { get; set; }
    }
}

