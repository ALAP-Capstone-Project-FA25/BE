namespace ALAP.Entity.DTO.Response
{
    public class RefundStatisticsDto
    {
        public int TotalTicketsNeedRefund { get; set; }
        public int TicketsRefunded { get; set; }
        public int TicketsPendingRefund { get; set; }
        public long TotalAmountNeedRefund { get; set; }
        public long TotalAmountRefunded { get; set; }
        public long TotalAmountPendingRefund { get; set; }
    }
}
