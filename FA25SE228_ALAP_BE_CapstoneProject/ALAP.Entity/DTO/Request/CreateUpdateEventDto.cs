using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.DTO.Request
{
    public class CreateUpdateEventDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string MeetingLink { get; set; } = string.Empty;
        public int CommissionRate { get; set; }
        public string ImageUrls { get; set; } = string.Empty;
        public long Amount { get; set; }
        public EventStatus Status { get; set; } = EventStatus.IN_PROGRESS;
        public long SpeakerId { get; set; }

        public string VideoUrl { get; set; }

    }
}
