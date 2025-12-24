using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.DTO.Request
{
    public class CreateUpdateCourseDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Price { get; set; }
        public int SalePrice { get; set; }
        public long MentorId { get; set; }
        public long CategoryId { get; set; }
        public long? SubjectId { get; set; }
        public CourseType CourseType { get; set; }
        public int Difficulty { get; set; } // 1-5 stars
    }
}
