using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFU.Common.Models
{
    public class UserViewModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public Gender Gender { get; set; }

        #region Personal Info

        public DateTime Birthday { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string CurrentPlan { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentificationNumber { get; set; }
        public DateTime? IdentificationCreatedTime { get; set; }
        public string IdentificationProvinceCode { get; set; }
        public string IdentificationFrontUrl { get; set; }
        public string IdentificationBackUrl { get; set; }
        public string Street { get; set; }
        public string WardCode { get; set; }
        public string DistrictCode { get; set; }
        public string ProvinceCode { get; set; }
        public string IsVerified { get; set; }
        public string UpdatedDate { get; set; }
        public string WebsiteUrl { get; set; }
        public string BannerUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string TikTokUrl { get; set; }
        public string AboutMe { get; set; }
        #endregion

        public UserViewModel()
        {

        }
    }
    public enum Gender
    {
        None = 0,
        Man = 1,
        Woman,
        Other
    }
}
