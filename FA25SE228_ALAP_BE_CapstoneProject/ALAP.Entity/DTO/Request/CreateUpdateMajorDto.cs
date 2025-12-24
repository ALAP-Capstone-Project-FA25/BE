namespace ALAP.Entity.DTO.Request
{
    public class CreateUpdateMajorDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

