using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.Models.Wapper
{
    public class PagingModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; }

        public long CourseId { get; set; }
        public long UserId { get; set; }
        public UserRole UserRole { get; set; }
        public long MajorId { get; set; } = 0;
    }
}
