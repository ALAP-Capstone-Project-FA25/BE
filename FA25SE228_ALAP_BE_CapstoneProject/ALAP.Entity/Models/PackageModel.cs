using ALAP.Entity.Common;
using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.Models
{
    public class PackageModel : BaseEntity
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public PackageType PackageType { get; set; } = PackageType.STARTER;
        public int Price { get; set; } = 0;
        public int Duration { get; set; } = 0; 
        public bool IsActive { get; set; } = true;
        public string Features { get; set; } = "";
        public bool IsPopular { get; set; } = false;

    }
}
