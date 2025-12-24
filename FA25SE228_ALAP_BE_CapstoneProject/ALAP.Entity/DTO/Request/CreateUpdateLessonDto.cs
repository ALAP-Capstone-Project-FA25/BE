namespace ALAP.Entity.DTO.Request
{
    public class CreateUpdateLessonDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string VideoUrl { get; set; }
        public int Duration { get; set; }
        public int OrderIndex { get; set; }
        public bool IsFree { get; set; } = false;
        public long TopicId { get; set; }
        public int LessonType { get; set; } = 1;
        public string? DocumentUrl { get; set; }
        public string? DocumentContent { get; set; }
    }
}
