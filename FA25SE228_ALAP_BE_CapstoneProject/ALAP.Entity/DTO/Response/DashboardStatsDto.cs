namespace ALAP.Entity.DTO.Response
{
    public class DashboardStatsDto
    {
        public int TotalCourses { get; set; }
        public string CourseGrowth { get; set; } = "+0%";
        public int TotalStudents { get; set; }
        public string StudentGrowth { get; set; } = "+0%";
        public decimal TotalRevenue { get; set; }
        public string RevenueGrowth { get; set; } = "+0%";
        public int AverageCompletionRate { get; set; }
        public string CompletionRateGrowth { get; set; } = "+0%";
    }

    public class RevenueChartDto
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<decimal> Data { get; set; } = new List<decimal>();
    }

    public class StudentGrowthChartDto
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<int> Data { get; set; } = new List<int>();
    }

    public class CourseDistributionDto
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<int> Data { get; set; } = new List<int>();
    }

    public class TopCourseDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CompletionRate { get; set; }
        public int StudentCount { get; set; }
    }

    public class RecentCourseDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public int Students { get; set; }
        public decimal Rating { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string Status { get; set; } = "active";
        public string Thumbnail { get; set; } = "🎓";
        public int Progress { get; set; }
        public string Revenue { get; set; } = "₫0";
    }
}