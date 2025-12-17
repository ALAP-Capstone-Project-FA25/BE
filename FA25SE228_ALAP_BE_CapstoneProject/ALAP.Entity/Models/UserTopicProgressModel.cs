using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace App.Entity.Models
{
    [Table("UserTopicProgresses")]
    public class UserTopicProgressModel : BaseEntity
    {
        [ForeignKey("UserTopic")]
        public long UserTopicId { get; set; }

        public int Score { get; set; } = 0;
        public int Version { get; set; } = 1;

        [JsonIgnore]
        public virtual UserTopicModel UserTopic { get; set; } = null;
    }
}


