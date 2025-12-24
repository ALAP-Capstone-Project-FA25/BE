namespace ALAP.Entity.DTO.Response
{
    public class AdaptiveCourseRecommendationDto
    {
        public long CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseDescription { get; set; }
        public string ImageUrl { get; set; }
        public int RecommendationScore { get; set; }
        public string RecommendationReason { get; set; }
        public int WeakLessonCount { get; set; }
        public double AverageMasteryLevel { get; set; }
        public bool IsEnrolled { get; set; }
        public List<WeakAreaSummaryDto> WeakAreas { get; set; } = new List<WeakAreaSummaryDto>();
    }

    public class WeakAreaSummaryDto
    {
        public long LessonId { get; set; }
        public string LessonTitle { get; set; }
        public int ReferralCount { get; set; }
        public int MasteryLevel { get; set; }
    }
}

