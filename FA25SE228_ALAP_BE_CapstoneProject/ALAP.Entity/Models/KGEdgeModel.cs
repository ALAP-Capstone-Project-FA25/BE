using ALAP.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Entity.Models
{
    public class KGEdgeModel : BaseEntity
    {
        public long EdgeId { get; set; }
        public long SubjectId { get; set; }
        public string EdgeCode { get; set; } = string.Empty;

        public long SourceNodeId { get; set; }
        public long TargetNodeId { get; set; }
        public string EdgeType { get; set; } = "required";

        // Navigation
        public virtual KGSubjectModel? Subject { get; set; }
        public virtual KGNodeModel? SourceNode { get; set; }
        public virtual KGNodeModel? TargetNode { get; set; }
    }
}
