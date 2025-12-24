using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.DTO.Response
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }
        public string Avatar { get; set; }
        public UserRole Role { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}
