using ALAP.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace App.Entity.Models
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
