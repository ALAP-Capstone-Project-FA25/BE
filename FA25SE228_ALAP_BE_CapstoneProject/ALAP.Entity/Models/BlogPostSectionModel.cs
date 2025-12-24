using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("BlogPostSections")]
    public class BlogPostSectionModel : BaseEntity
    {
        [Required]
        [StringLength(500)]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "LONGTEXT")]
        public string Content { get; set; }

        public int OrderIndex { get; set; }

        [ForeignKey("BlogPost")]
        public long BlogPostId { get; set; }

        [JsonIgnore]
        public virtual BlogPostModel BlogPost { get; set; } = null!;
    }
}
