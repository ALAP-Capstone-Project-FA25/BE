namespace ALAP.Entity.DTO.Response
{
    public class CancelEventResultDto
    {
        public bool Success { get; set; }
        public int TicketsMarkedForRefund { get; set; }
        public string Message { get; set; }
    }
}
