using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.DTO.Response
{
    public class BlogPostResponseDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string? ImageUrl { get; set; }
        public BlogPostTargetAudience TargetAudience { get; set; }
        public long AuthorId { get; set; }
        public string? Tags { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Include related entities
        public UserResponseDTO? Author { get; set; }
        public List<BlogPostSectionResponseDto> Sections { get; set; } = new List<BlogPostSectionResponseDto>();
        public List<BlogPostCommentResponseDto> Comments { get; set; } = new List<BlogPostCommentResponseDto>();
        public List<BlogPostLikeResponseDto> Likes { get; set; } = new List<BlogPostLikeResponseDto>();
    }

    public class BlogPostSectionResponseDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int OrderIndex { get; set; }
        public long BlogPostId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class BlogPostCommentResponseDto
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public long UserId { get; set; }
        public long BlogPostId { get; set; }
        public long? ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public UserResponseDTO? User { get; set; }
        public List<BlogPostCommentResponseDto> Replies { get; set; } = new List<BlogPostCommentResponseDto>();
    }

    public class BlogPostLikeResponseDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long BlogPostId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
