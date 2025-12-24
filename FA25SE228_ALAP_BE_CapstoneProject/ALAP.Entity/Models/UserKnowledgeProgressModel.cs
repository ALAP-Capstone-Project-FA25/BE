using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALAP.Entity.Models
{
    [Table("UserKnowledgeProgress")]
    public class UserKnowledgeProgressModel : BaseEntity
    {
        [ForeignKey("User")]
        public long UserId { get; set; }
        
        [ForeignKey("KGNode")]
        public long NodeId { get; set; }
        
        public long SubjectId { get; set; }
        
        // Progress tracking
        public string Status { get; set; } = "locked"; // locked, available, in-progress, completed
        public int ProgressPercent { get; set; } = 0;
        
        // Timestamps
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? LastAccessedAt { get; set; }
        
        // Related courses
        public string? CompletedCourseIds { get; set; } // JSON array of course IDs
        
        // Navigation
        public virtual UserModel User { get; set; } = null!;
        public virtual KGNodeModel Node { get; set; } = null!;
    }
}
