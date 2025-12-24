using ALAP.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ALAP.Entity.Models
{
    [Table("Courses")]
    public class CourseModel : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        public string ImageUrl { get; set; }

        public int SalePrice { get; set; }
        public int Members { get; set; }

        [ForeignKey("Category")]    
        public long CategoryId { get; set; }

        [JsonIgnore]
        public virtual CategoryModel Category { get; set; } = null;

        public virtual ICollection<TopicModel> Topics { get; set;  } = new List<TopicModel>();
        public virtual List<UserCourseModel> UserCourses { get; set; } = new List<UserCourseModel>();

        [ForeignKey("Mentor")]
        public long MentorId { get; set; }
        
        public UserModel Mentor { get; set; }

        [JsonIgnore]
        public List<ChatRoomModel> ChatRooms { get; set; } = new List<ChatRoomModel>();

       
    }
}
