using System.ComponentModel.DataAnnotations;

namespace ALAP.Entity.DTO.Request
{
    public class UserLoginRequestDTO
    {
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password not Empty")]
        public string Password { get; set; }
    }
}
