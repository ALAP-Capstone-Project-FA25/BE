using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ALAP.Entity.Common;
using ALAP.Entity.Models.Enums;

namespace ALAP.Entity.Models
{
    [Table("users")]
    public class UserModel : BaseEntity
    {

        [StringLength(100, ErrorMessage = "First Name must be less than 100 characters.")]
        public string? FirstName { get; set; }

        [StringLength(100, ErrorMessage = "Last Name must be less than 100 characters.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, ErrorMessage = "Username must be less than 100 characters.")]
        public string Username { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(100, ErrorMessage = "Email must be less than 100 characters.")]
        public string? Email { get; set; }
        public long Balance { get; set; } = 0;
        public  Gender Gender { get; set; } = 0;

        public bool EmailConfirmed { get; set; } = false;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [JsonIgnore]
        public string? Password { get; set; }

        public string? Avatar { get; set; }
        public bool IsActive { get; set; } = true;

        [StringLength(100, ErrorMessage = "Phone must be less than 100 characters.")]
        public string? Phone { get; set; }

        [StringLength(255, ErrorMessage = "Province must be less than 100 characters.")]
        public string? Address { get; set; }

        public UserRole Role { get; set; } = UserRole.USER;

        public string? GoogleId { get; set; } = string.Empty;
        public UserOrigin UserOrigin { get; set; } = UserOrigin.System;

        [ForeignKey("MajorModel")]
        public long? MajorId { get; set; } = null;

        public virtual MajorModel MajorModel { get; set; } = null;

        public virtual ICollection<LoginHistoryModel> LoginHistories { get; set; } = new List<LoginHistoryModel>();
        public virtual ICollection<UserCourseModel> UserCourses { get; set; } = new List<UserCourseModel>();
        public virtual ICollection<UserPackageModel> UserPackages { get; set; } = new List<UserPackageModel>();


    }
}
