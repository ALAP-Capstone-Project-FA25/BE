using ALAP.Entity.Common;
using ALAP.Entity.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("notifications")]
    public class NotificationModel : BaseEntity
    {
        [Required]
        [ForeignKey("User")]
        public long UserId { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Message { get; set; }

        [StringLength(500)]
        public string? LinkUrl { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime? ReadAt { get; set; }

        // Additional data as JSON for flexibility
        [Column(TypeName = "LONGTEXT")]
        public string? Metadata { get; set; }

        [JsonIgnore]
        public virtual UserModel User { get; set; } = null!;
    }
}
