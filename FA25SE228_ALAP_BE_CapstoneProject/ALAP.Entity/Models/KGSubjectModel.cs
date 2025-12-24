using ALAP.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.Entity.Models
{
    public class KGSubjectModel : BaseEntity
    {
        [ForeignKey("Subject")]
        public long SubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Version { get; set; } = "1.0";
        public DateTime? ExportedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public virtual ICollection<KGNodeModel> Nodes { get; set; } = [];
        public virtual ICollection<KGEdgeModel> Edges { get; set; } = [];

        public CategoryModel Subject { get; set; } = null!;

    }
}
