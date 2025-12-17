using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace App.Entity.Models
{
    [Table("EntryTestSubjectMappings")]
    public class EntryTestSubjectMappingModel : BaseEntity
    {
        [ForeignKey("Option")]
        public long OptionId { get; set; }
        
        [ForeignKey("Category")]
        public long CategoryId { get; set; }
        
        public int Weight { get; set; } = 1; // Trọng số cho môn học

        [JsonIgnore]
        public virtual EntryTestOptionModel Option { get; set; } = null!;
        
        public virtual CategoryModel Category { get; set; } = null!;
    }
}
