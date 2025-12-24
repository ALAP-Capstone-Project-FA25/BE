namespace ALAP.Entity.DTO.Request
{
    public class CreateUpdateLoginHistoryDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime LoginDate { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}

