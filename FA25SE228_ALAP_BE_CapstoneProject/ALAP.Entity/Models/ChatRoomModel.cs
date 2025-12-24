using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALAP.Entity.Models
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
