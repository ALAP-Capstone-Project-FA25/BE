using ALAP.Entity.Models.Enums;
using Base.Common;

namespace ALAP.Entity.DTO.Request
{
    public class CreateUpdateChatRoomMessageDto
    {
        public long Id { get; set; }
        public long ChatRoomId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string MessageLink { get; set; } = string.Empty;
        public bool IsUser { get; set; }
        public MessageType MessageType { get; set; } = MessageType.TEXT;
        public bool IsRead { get; set; }
        public int Message { get; set; }
        public DateTime CreatedAt { get; set; } = Utils.GetCurrentVNTime();
    }
}

