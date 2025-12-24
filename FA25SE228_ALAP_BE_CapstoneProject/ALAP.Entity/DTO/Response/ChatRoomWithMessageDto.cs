using ALAP.Entity.Models;

namespace ALAP.Entity.DTO.Response
{
    public class ChatRoomWithMessageDto
    {
        public ChatRoomModel ChatRoom { get; set; }
        public List<ChatRoomMessageModel> Messages { get; set; }
    }
}
