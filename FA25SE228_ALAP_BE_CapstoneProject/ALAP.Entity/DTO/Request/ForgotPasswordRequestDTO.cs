using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.Entity.DTO.Request
{
    public class ForgotPasswordRequestDTO
    {
        [Required(ErrorMessage = "The email field is required.")]
        [EmailAddress(ErrorMessage = "The email field must be a valid email address.")]
        public string Email { get; set; }
    }
}
