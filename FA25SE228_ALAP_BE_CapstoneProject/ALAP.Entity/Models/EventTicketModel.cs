using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALAP.Entity.Models
{
    public class EventTicketModel : BaseEntity
    {
        [ForeignKey("Event")]
        public long EventId { get; set; }
        [ForeignKey("Payment")]
        public long PaymentId { get; set; }
        [ForeignKey("User")]
        public long UserId { get; set; }

        public bool IsActive { get; set; } = false;

        public long Amount { get; set; }

        public bool NeedRefund { get; set; } = false;
        public bool IsRefunded { get; set; } = false;
        public string RefundImageUrl { get; set; } = string.Empty;
        
        public virtual EventModel Event { get; set; }
        public virtual UserModel User { get; set; }
        public virtual PaymentModel Payment { get; set; }

    }
}
