using ALAP.Entity.Common;
using ALAP.Entity.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("BlogPosts")]
    public class BlogPostModel : BaseEntity
    {
        [Required]
        [StringLength(500)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string? ImageUrl { get; set; }

        public BlogPostTargetAudience TargetAudience { get; set; }

        [ForeignKey("Author")]
        public long AuthorId { get; set; }

        [Column(TypeName = "LONGTEXT")]
        public string? Tags { get; set; } // JSON array of strings

        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        public virtual UserModel Author { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<BlogPostSectionModel> Sections { get; set; } = new List<BlogPostSectionModel>();

        [JsonIgnore]
        public virtual ICollection<BlogPostCommentModel> Comments { get; set; } = new List<BlogPostCommentModel>();

        [JsonIgnore]
        public virtual ICollection<BlogPostLikeModel> Likes { get; set; } = new List<BlogPostLikeModel>();
    }
}
