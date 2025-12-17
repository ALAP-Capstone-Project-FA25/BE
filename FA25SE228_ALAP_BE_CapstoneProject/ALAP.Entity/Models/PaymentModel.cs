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
    public class PaymentModel : BaseEntity
    {
        public long Code { get; set; }
        public string? Item { get; set; }

        public int Amount { get; set; }
        public string QrCode { get; set; } = "";
        public string PaymentUrl { get; set; } = "";

        //public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddMinutes(15);

        public PaymentType PaymentType { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.PENDING;

        [ForeignKey("Package")]
        public long? PackageId { get; set; }
        public PackageModel? Package { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        public virtual UserModel User { get; set; }


        [JsonIgnore]
        public EventTicketModel EventTicket { get; set; } = null!;
    }
}
