namespace ALAP.Entity.DTO.Request
{
    public class ImportKnowledgeGraphDto
    {
        public long SubjectId { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public string? Description { get; set; }
        public List<ImportNodeDto> Nodes { get; set; } = new();
        public List<ImportEdgeDto> Edges { get; set; } = new();
    }

    public class ImportNodeDto
    {
        public string? Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string? Label { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public List<string> Concepts { get; set; } = new();
        public List<string> Examples { get; set; } = new();
        public List<string> Prerequisites { get; set; } = new();
        public string? EstimatedTime { get; set; }
        public string? Difficulty { get; set; }
        public List<ImportResourceDto> Resources { get; set; } = new();
    }

    public class ImportResourceDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public long? CourseId { get; set; }
    }

    public class ImportEdgeDto
    {
        public string? Id { get; set; }
        public string? Source { get; set; }
        public string? Target { get; set; }
        public string? Type { get; set; } = "required";
    }
}
