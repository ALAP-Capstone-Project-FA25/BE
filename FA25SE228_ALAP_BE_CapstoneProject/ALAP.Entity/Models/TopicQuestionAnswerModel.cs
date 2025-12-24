using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    public class TopicQuestionAnswerModel : BaseEntity
    {
        [ForeignKey("TopicQuestion")]
        public long TopicQuestionId { get; set; }
        public string Answer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;

        [JsonIgnore]
        public virtual TopicQuestionModel TopicQuestion { get; set; } = null!;
    }
}
