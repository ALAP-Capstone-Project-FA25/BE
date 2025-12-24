namespace ALAP.Entity.DTO.Request
{
    public class CreateUpdateCategoryDto
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public long MajorId { get; set; }
        public string Name { get; set; }
    }
}
