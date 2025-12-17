using ALAP.Entity.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace App.Entity.Models
{
    [Table("EntryTestResults")]
    public class EntryTestResultModel : BaseEntity
    {
        [ForeignKey("User")]
        public long UserId { get; set; }
        
        [ForeignKey("EntryTest")]
        public long EntryTestId { get; set; }
        
        public string AnswersJson { get; set; } = string.Empty; // JSON: {"1":"A","2":"B",...}
        public string RecommendedSubjectsJson { get; set; } = string.Empty; // JSON: [{"categoryId":1,"score":5}]
        public string RecommendedMajorsJson { get; set; } = string.Empty; // JSON: [{"majorId":1,"score":10}]
        public DateTime CompletedAt { get; set; }

        [JsonIgnore]
        public virtual UserModel User { get; set; } = null!;
        
        [JsonIgnore]
        public virtual EntryTestModel EntryTest { get; set; } = null!;
    }
}
