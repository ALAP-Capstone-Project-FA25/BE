using ALAP.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALAP.Entity.Models
{
    public class LessonNoteModel : BaseEntity
    {
        public string Text { get; set; }
        public int Time { get; set; }

        [ForeignKey("Lesson")]
        public long LessonId { get; set; }

        [JsonIgnore]
        public virtual LessonModel Lesson {get;set;}
    }
}
