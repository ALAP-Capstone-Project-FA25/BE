using System.ComponentModel.DataAnnotations;

namespace ALAP.Entity.DTO.Request.User
{
    public class UpdateProfileDTO
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string? Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }
    }
}