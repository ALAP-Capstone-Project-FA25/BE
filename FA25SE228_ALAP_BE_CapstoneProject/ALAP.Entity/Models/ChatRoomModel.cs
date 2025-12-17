using ALAP.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace App.Entity.Models
{
    public class ChatRoomModel : BaseEntity
    {
        public string Name { get; set; }

        [ForeignKey("CreatedBy")]
        public long CreatedById { get; set; }

        [ForeignKey("Participant")]
        public long ParticipantId { get; set; }

        [ForeignKey("Course")]
        public long? CourseId { get; set; } = null;
        public virtual CourseModel Course { get; set; } = null;

        public virtual UserModel CreatedBy { get; set; } = null;

        public virtual UserModel Participant { get; set; } = null;

        public virtual ICollection<ChatRoomMessageModel> Messages { get; set; } = new List<ChatRoomMessageModel>();

    }
}
