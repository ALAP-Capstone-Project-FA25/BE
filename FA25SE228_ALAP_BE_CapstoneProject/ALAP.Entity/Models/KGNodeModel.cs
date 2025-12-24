using ALAP.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.Entity.Models
{
    public class KGNodeModel : BaseEntity
    {
        public long NodeId { get; set; }
        public long SubjectId { get; set; }
        public string NodeCode { get; set; } = string.Empty;

        // Position
        public double PositionX { get; set; }
        public double PositionY { get; set; }

        // Data
        public string Label { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? EstimatedTime { get; set; }
        public string Difficulty { get; set; } = "Cơ bản";
        public string Status { get; set; } = "locked";

        // JSON columns
        public string ConceptsJson { get; set; } = "[]";
        public string ExamplesJson { get; set; } = "[]";
        public string PrerequisitesJson { get; set; } = "[]";
        public string ResourcesJson { get; set; } = "[]";

        // Navigation
        public virtual KGSubjectModel? Subject { get; set; }
        public virtual ICollection<KGEdgeModel> OutgoingEdges { get; set; } = [];
        public virtual ICollection<KGEdgeModel> IncomingEdges { get; set; } = [];
    }
}
