namespace ALAP.Entity.DTO.Response
{
    public class SearchResultDto
    {
        public List<CourseSearchResultDto> Courses { get; set; } = new();
        public List<LessonSearchResultDto> Lessons { get; set; } = new();
        public List<TopicSearchResultDto> Topics { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class CourseSearchResultDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }

    public class LessonSearchResultDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public string TopicTitle { get; set; } = string.Empty;
        public long CourseId { get; set; }
        public long TopicId { get; set; }
    }

    public class TopicSearchResultDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public long CourseId { get; set; }
    }
}
