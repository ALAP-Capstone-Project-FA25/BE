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
    [Table("Topics")]
    public class TopicModel : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int OrderIndex { get; set; }

        [ForeignKey("Course")]
        public long CourseId { get; set; }

        [JsonIgnore]
        public virtual CourseModel Course { get; set; } = null;

        public virtual ICollection<LessonModel> Lessons { get; set; } = new List<LessonModel>();
    
        public virtual ICollection<TopicQuestionModel> TopicQuestions { get; set; } = new List<TopicQuestionModel>();

    }
}