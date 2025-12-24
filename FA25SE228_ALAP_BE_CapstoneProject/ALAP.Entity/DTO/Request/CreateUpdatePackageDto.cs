using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.DTO.Request
{
    public class CreateUpdatePackageDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PackageType PackageType { get; set; } = PackageType.STARTER;
        public int Price { get; set; } = 0;
        public int Duration { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}


