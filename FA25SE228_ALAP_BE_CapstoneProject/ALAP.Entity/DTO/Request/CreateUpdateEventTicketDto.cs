namespace App.Entity.DTO.Request
{
    public class CreateUpdateEventTicketDto
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public long UserId { get; set; }
        public long Amount { get; set; }
    }
}
