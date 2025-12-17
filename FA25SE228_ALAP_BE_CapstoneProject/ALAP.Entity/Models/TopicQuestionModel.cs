using ALAP.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace App.Entity.Models
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
