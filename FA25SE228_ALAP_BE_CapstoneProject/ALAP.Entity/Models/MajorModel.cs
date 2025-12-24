using ALAP.Entity.Common;

namespace ALAP.Entity.Models
{
    public class MajorModel : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        
        public virtual ICollection<CategoryModel> Categories { get; set; } 
    }
}
