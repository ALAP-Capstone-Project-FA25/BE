using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.DTO.Response
{
    public class UserLessonDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string VideoUrl { get; set; }
        public int Duration { get; set; }
        public int OrderIndex { get; set; }
        public bool IsFree { get; set; } = false;
        public bool IsCurrent { get; set; }
        public LessonType LessonType { get; set; } = LessonType.VIDEO;
        public string? DocumentUrl { get; set; }
        public string? DocumentContent { get; set; }
        public long TopicId { get; set; }
    }
}
