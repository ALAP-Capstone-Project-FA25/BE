namespace ALAP.Entity.DTO.Response
{
    public class EntryTestResultDto
    {
        public List<SubjectRecommendationDto> RecommendedSubjects { get; set; } = [];
        public List<MajorRecommendationDto> RecommendedMajors { get; set; } = [];
    }

    public class SubjectRecommendationDto
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Score { get; set; }
    }

    public class MajorRecommendationDto
    {
        public long MajorId { get; set; }
        public string MajorName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Score { get; set; }
        public List<string> RelatedSubjects { get; set; } = [];
    }
}
