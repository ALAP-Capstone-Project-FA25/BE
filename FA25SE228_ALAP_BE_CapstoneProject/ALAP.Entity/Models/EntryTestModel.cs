using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALAP.Entity.Models
{
    [Table("EntryTests")]
    public class EntryTestModel : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;

        public virtual ICollection<EntryTestQuestionModel> Questions { get; set; } = [];
    }
}
