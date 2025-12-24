using ALAP.Entity.Common;
using Base.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("login_history")]
    public class LoginHistoryModel : BaseEntity
    {
        [Required]
        [ForeignKey("User")]
        public long UserId { get; set; }

        [Required]
        public DateTime LoginDate { get; set; } = Utils.GetCurrentVNTime();

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }



        [JsonIgnore]
        public virtual UserModel User { get; set; } = null!;
    }
}

