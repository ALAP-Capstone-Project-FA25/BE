using System.ComponentModel.DataAnnotations;

namespace ALAP.Entity.DTO.Request.User
{
    public class CreateMentorRequestDTO
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "First Name must be less than 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, ErrorMessage = "Last Name must be less than 100 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(100, ErrorMessage = "Email must be less than 100 characters")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "Phone must be less than 100 characters")]
        public string? Phone { get; set; }

        [StringLength(255, ErrorMessage = "Address must be less than 255 characters")]
        public string? Address { get; set; }

        public string? Avatar { get; set; }

        [StringLength(500, ErrorMessage = "Bio must be less than 500 characters")]
        public string? Bio { get; set; }
    }
}
