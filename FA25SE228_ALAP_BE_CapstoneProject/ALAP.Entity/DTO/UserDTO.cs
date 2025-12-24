using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.DTO
{
    public class UserDTO
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string Username { get; set; }


        public string? Email { get; set; }

        public Gender Gender { get; set; } = 0;

        public bool EmailConfirmed { get; set; } = false;
    }
}
