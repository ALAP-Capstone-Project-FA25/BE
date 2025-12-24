using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("Categories")]
    public class CategoryModel : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }


        [ForeignKey("Major")]
        public long MajorId { get; set; }

        [JsonIgnore]
        public virtual MajorModel Major { get; set; }

        [JsonIgnore]
        public virtual ICollection<CourseModel> Courses { get; set; }
    }
}
