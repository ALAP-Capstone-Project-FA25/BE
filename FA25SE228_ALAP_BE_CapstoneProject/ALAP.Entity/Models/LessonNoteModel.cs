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
