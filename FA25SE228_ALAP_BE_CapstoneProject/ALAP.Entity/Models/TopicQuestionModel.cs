using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    public class TopicQuestionModel : BaseEntity
    {
        [ForeignKey("Topic")]
        public long TopicId { get; set; }
        public string Question { get; set; } = string.Empty;
        public int MaxChoices { get; set; } = 1;

        [JsonIgnore]
        public virtual TopicModel Topic { get; set; } = null!;

        public virtual ICollection<TopicQuestionAnswerModel> TopicQuestionAnswers { get; set; } = new List<TopicQuestionAnswerModel>();
    }
}
