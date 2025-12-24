using ALAP.Entity.Models;

namespace ALAP.Entity.DTO
{
    public class CategoryDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public long MajorId { get; set; }
        public List<CourseModel> Courses { get; set; }
    }
}
