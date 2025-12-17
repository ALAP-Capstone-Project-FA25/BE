using ALAP.Entity.Common;
using ALAP.Entity.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace App.Entity.Models
{
    public class EventModel : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string VideoUrl { get; set; }

        public bool IsPaidForSpeaker { get; set; } = false;
        public string PaymentProofImageUrl { get; set; } = string.Empty;

        public string MeetingLink { get; set; } = string.Empty;
        public int CommissionRate { get; set; }
        public string ImageUrls { get; set; } = string.Empty;
        public long Amount { get; set; }
        public EventStatus Status { get; set; } = EventStatus.IN_PROGRESS;

        [ForeignKey("Speaker")]
        public long SpeakerId { get; set; }
        public virtual UserModel Speaker { get; set; }

        [JsonIgnore]
        public ICollection<EventTicketModel> EventTickets { get; set; } = new List<EventTicketModel>();

    }
}
