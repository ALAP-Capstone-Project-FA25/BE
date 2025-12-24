using ALAP.Entity.Models;
using ALAP.Entity.Models.Enums;
using Base.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALAP.Entity.DTO.Response
{
    public class CourseDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        public string ImageUrl { get; set; }

        public int SalePrice { get; set; }
        public int Members { get; set; }

        public CourseType CourseType { get; set; }
        public int Difficulty { get; set; } // 1-5 stars

        [ForeignKey("Category")]
        public long CategoryId { get; set; }

        public virtual CategoryModel Category { get; set; } = null;

        public virtual ICollection<TopicModel> Topics { get; set; } = new List<TopicModel>();
        public virtual List<UserCourseResponseDto> UserCourses { get; set; } = new List<UserCourseResponseDto>();

        [ForeignKey("Mentor")]
        public long MentorId { get; set; }

        public UserModel Mentor { get; set; }

        public DateTime CreatedAt { get; set; } = Utils.GetCurrentVNTime();

        public DateTime UpdatedAt { get; set; }
    }
}
