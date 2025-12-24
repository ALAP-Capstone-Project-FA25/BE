using System.ComponentModel.DataAnnotations;
using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.DTO.Request
{
    public class RegisterDTO
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(250)]
        public string? PasswordHash { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? Avatar { get; set; }

        [MaxLength(10)]
        [Phone]
        public string? Phone { get; set; }

        public Gender Gender { get; set; }

    }
}
