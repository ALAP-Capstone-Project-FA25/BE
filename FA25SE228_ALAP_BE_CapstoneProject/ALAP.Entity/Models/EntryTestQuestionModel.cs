using ALAP.Entity.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("EntryTestQuestions")]
    public class EntryTestQuestionModel : BaseEntity
    {
        [ForeignKey("EntryTest")]
        public long EntryTestId { get; set; }
        
        public string QuestionText { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;

        [JsonIgnore]
        public virtual EntryTestModel EntryTest { get; set; } = null!;
        
        public virtual ICollection<EntryTestOptionModel> Options { get; set; } = [];
    }
}
