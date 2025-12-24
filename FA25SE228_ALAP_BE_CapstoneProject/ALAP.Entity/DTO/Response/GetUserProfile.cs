using ALAP.Entity.Models.Enums;

namespace ALAPALAP.Entity.DTO.Response
{
    public class GetUserProfile
    {
        public long Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;
        public string? Avatar { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreateAt { get; set; }
        public Gender Gender { get; set; }
        public List<string> Roles { get; set; }
    }
}
