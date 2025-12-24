using ALAP.Entity.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ALAP.Entity.DTO.Request
{
    public class CreateUpdateBlogPostDto
    {
        public long Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? ImageUrl { get; set; }

        public BlogPostTargetAudience TargetAudience { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public List<BlogPostSectionDto> Sections { get; set; } = new List<BlogPostSectionDto>();
    }

    public class BlogPostSectionDto
    {
        public long Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public int OrderIndex { get; set; }
    }
}
