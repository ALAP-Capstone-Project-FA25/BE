namespace ALAP.Entity.DTO.Request
{
    public class CreateUpdateChatRoomDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long CreatedById { get; set; }
        public long ParticipantId { get; set; }
    }
}

