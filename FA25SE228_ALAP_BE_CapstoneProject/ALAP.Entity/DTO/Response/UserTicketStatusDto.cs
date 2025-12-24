using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.DTO.Response
{
    public class UserTicketStatusDto
    {
        public bool HasTicket { get; set; }
        public long? TicketId { get; set; }
        public long? PaymentId { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public string PaymentUrl { get; set; }
        public long? Amount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsExpired { get; set; }
        public int? MinutesRemaining { get; set; }
    }
}
