using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.DTO.Request
{
    public class BlogPostFilterDto
    {
        public BlogPostTargetAudience? TargetAudience { get; set; }
        public string? Tag { get; set; }
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsActive { get; set; } // For admin filtering
    }
}
