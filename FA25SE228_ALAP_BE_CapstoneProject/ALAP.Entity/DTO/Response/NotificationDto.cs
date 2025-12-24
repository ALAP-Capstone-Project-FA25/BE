using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.DTO.Response
{
    public class NotificationDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string? LinkUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Metadata { get; set; }
    }
}
