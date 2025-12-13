using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Common;

namespace ALAP.Entity.Common
{
    public class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime CreatedAt { get; set; } = Utils.GetCurrentVNTime();

        public DateTime UpdatedAt { get; set; }
    }
}
