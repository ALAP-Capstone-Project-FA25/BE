namespace ALAP.Entity.DTO.Request
{
    public class UpdateRefundDto
    {
        public long TicketId { get; set; }
        public string RefundImageUrl { get; set; }
        public bool IsRefunded { get; set; }
    }
}
