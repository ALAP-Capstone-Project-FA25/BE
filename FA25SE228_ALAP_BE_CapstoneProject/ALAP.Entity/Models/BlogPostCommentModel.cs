using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("BlogPostComments")]
    public class BlogPostCommentModel : BaseEntity
    {
        [Required]
        [Column(TypeName = "LONGTEXT")]
        public string Content { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        [ForeignKey("BlogPost")]
        public long BlogPostId { get; set; }

        [ForeignKey("ParentComment")]
        public long? ParentCommentId { get; set; }

        [JsonIgnore]
        public virtual UserModel User { get; set; } = null!;

        [JsonIgnore]
        public virtual BlogPostModel BlogPost { get; set; } = null!;

        [JsonIgnore]
        public virtual BlogPostCommentModel? ParentComment { get; set; }

        [JsonIgnore]
        public virtual ICollection<BlogPostCommentModel> Replies { get; set; } = new List<BlogPostCommentModel>();
    }
}
