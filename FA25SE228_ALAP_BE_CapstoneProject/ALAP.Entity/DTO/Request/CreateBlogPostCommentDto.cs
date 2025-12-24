using System.ComponentModel.DataAnnotations;

namespace ALAP.Entity.DTO.Request
{
    public class CreateBlogPostCommentDto
    {
        [Required]
        public long BlogPostId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public long? ParentCommentId { get; set; }
    }
}
