using ALAP.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ALAP.Entity.Models
{
    public class UserPackageModel : BaseEntity
    {
        [ForeignKey("User")]
        public long UserId { get; set; }

        [ForeignKey("Package")]
        public long PackageId { get; set; }
        public DateTime ExpiredAt { get; set; }

        [ForeignKey("Payment")]
        public long PaymentId { get; set; }
        public bool IsActive { get; set; } = true;
        [JsonIgnore]

        public virtual UserModel User { get; set; }

        [JsonIgnore]
        public virtual PackageModel Package { get; set; }

        [JsonIgnore]
        public virtual PaymentModel Payment { get; set; }
    }
}
