using ALAP.Entity.Common;
using ALAP.Entity.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ALAP.Entity.Models
{
    [Table("Lessons")]
    public class LessonModel : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string VideoUrl { get; set; }
        public int Duration { get; set; } 
        public int OrderIndex { get; set; }
        public bool IsFree { get; set; } = false;
        public LessonType LessonType { get; set; } = LessonType.VIDEO;
        public string? DocumentUrl { get; set; }
        public string? DocumentContent { get; set; }

        [ForeignKey("Topic")]
        public long TopicId { get; set; }

        [JsonIgnore]
        public virtual TopicModel Topic { get; set; } = null;

        public ICollection<LessonNoteModel> LessonNotes { get; set; } = new List<LessonNoteModel>();
    }
}