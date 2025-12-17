using System.Text.Json.Serialization;

namespace ALAP.Entity.DTO.KnowledgeGraph
{
    public class KGImportDto
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";

        [JsonPropertyName("exportedAt")]
        public DateTime ExportedAt { get; set; }

        [JsonPropertyName("nodes")]
        public List<KGNodeImportDto> Nodes { get; set; } = [];

        [JsonPropertyName("edges")]
        public List<KGEdgeImportDto> Edges { get; set; } = [];
    }

    public class KGNodeImportDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("position")]
        public KGPositionDto Position { get; set; } = new();

        [JsonPropertyName("data")]
        public KGNodeDataDto Data { get; set; } = new();
    }

    public class KGPositionDto
    {
        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }
    }

    public class KGNodeDataDto
    {
        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("concepts")]
        public List<string> Concepts { get; set; } = [];

        [JsonPropertyName("examples")]
        public List<string> Examples { get; set; } = [];

        [JsonPropertyName("prerequisites")]
        public List<string> Prerequisites { get; set; } = [];

        [JsonPropertyName("estimatedTime")]
        public string EstimatedTime { get; set; } = string.Empty;

        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = "locked";

        [JsonPropertyName("resources")]
        public List<KGResourceDto> Resources { get; set; } = [];
    }

    public class KGResourceDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("courseId")]
        public long? CourseId { get; set; }
    }

    public class KGEdgeImportDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("target")]
        public string Target { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = "required";
    }

    public class KGImportResultDto
    {
        public long SubjectId { get; set; }
        public int NodesImported { get; set; }
        public int EdgesImported { get; set; }
        public int NodesSkipped { get; set; }
        public int EdgesSkipped { get; set; }
        public List<string> Warnings { get; set; } = [];
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class KGExportDto
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";

        [JsonPropertyName("exportedAt")]
        public DateTime ExportedAt { get; set; }

        [JsonPropertyName("nodes")]
        public List<KGNodeImportDto> Nodes { get; set; } = [];

        [JsonPropertyName("edges")]
        public List<KGEdgeImportDto> Edges { get; set; } = [];
    }
}
