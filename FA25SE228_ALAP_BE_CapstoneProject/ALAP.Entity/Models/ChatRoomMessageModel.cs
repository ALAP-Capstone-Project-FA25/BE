using ALAP.Entity.Common;
using ALAP.Entity.Models.Enums;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    public class ChatRoomMessageModel : BaseEntity
    {
        public string Content { get; set; }
        public bool IsUser { get; set; }
        public bool IsRead { get; set; }
        public MessageType MessageType { get; set; } = MessageType.TEXT;
        public string MessageLink { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("ChatRoom")]
        public long ChatRoomId { get; set; }

        [JsonIgnore]
        public virtual ChatRoomModel ChatRoom { get; set; } = null!;
    }
}
