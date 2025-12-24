using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    [Table("EntryTestOptions")]
    public class EntryTestOptionModel : BaseEntity
    {
        [ForeignKey("Question")]
        public long QuestionId { get; set; }
        
        public string OptionCode { get; set; } = string.Empty; // A, B, C, D
        public string OptionText { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;

        [JsonIgnore]
        public virtual EntryTestQuestionModel Question { get; set; } = null!;
        
        public virtual ICollection<EntryTestSubjectMappingModel> SubjectMappings { get; set; } = [];
    }
}
