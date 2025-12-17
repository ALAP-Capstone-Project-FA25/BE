using ALAP.Entity.Common;
using ALAP.Entity.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace App.Entity.Models
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
