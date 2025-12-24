using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("BlogPostLikes")]
    public class BlogPostLikeModel : BaseEntity
    {
        [ForeignKey("User")]
        public long UserId { get; set; }

        [ForeignKey("BlogPost")]
        public long BlogPostId { get; set; }

        [JsonIgnore]
        public virtual UserModel User { get; set; } = null!;

        [JsonIgnore]
        public virtual BlogPostModel BlogPost { get; set; } = null!;
    }
}
