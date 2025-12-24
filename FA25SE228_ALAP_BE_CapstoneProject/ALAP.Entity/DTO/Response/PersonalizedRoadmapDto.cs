namespace ALAP.Entity.DTO.Response
{
    public class PersonalizedRoadmapDto
    {
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public List<RoadmapNodeDto> Nodes { get; set; } = new();
        public List<RoadmapEdgeDto> Edges { get; set; } = new();
        public RoadmapStatisticsDto Statistics { get; set; }
    }

    public class RoadmapNodeDto
    {
        public string Id { get; set; }
        public string Type { get; set; } = "knowledge";
        public PositionDto Position { get; set; }
        public NodeDataDto Data { get; set; }
    }

    public class PositionDto
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class NodeDataDto
    {
        public string Label { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public List<string> Concepts { get; set; } = new();
        public List<string> Examples { get; set; } = new();
        public List<string> Prerequisites { get; set; } = new();
        public string EstimatedTime { get; set; }
        public string Difficulty { get; set; }
        public string Status { get; set; } // completed, in-progress, locked, available
        public List<ResourceDto> Resources { get; set; } = new();
        public int ProgressPercent { get; set; }
        public bool HasPurchasedCourse { get; set; }
        public List<long> RelatedCourseIds { get; set; } = new();
    }

    public class ResourceDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public long? CourseId { get; set; }
        public bool IsPurchased { get; set; }
    }

    public class RoadmapEdgeDto
    {
        public string Id { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string Type { get; set; } = "required"; // required, optional
    }

    public class RoadmapStatisticsDto
    {
        public int TotalNodes { get; set; }
        public int CompletedNodes { get; set; }
        public int InProgressNodes { get; set; }
        public int AvailableNodes { get; set; }
        public int LockedNodes { get; set; }
        public int CompletionPercent { get; set; }
        public int TotalCoursesPurchased { get; set; }
        public int TotalCoursesCompleted { get; set; }
    }
}
