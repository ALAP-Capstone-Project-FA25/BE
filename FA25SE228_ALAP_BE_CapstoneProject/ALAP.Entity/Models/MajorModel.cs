using ALAP.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.Entity.Models
{
    public class MajorModel : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        
        public virtual ICollection<CategoryModel> Categories { get; set; } 
    }
}
