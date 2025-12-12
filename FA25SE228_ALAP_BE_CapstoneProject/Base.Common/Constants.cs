namespace Base.Common
{
    public class Constants
    {
        #region Common
        public const string GetDataSuccess = "Get data succeeded.";
        public const string GetDataFailed = "Get data failed.";
        public const string SaveDataSuccess = "Lưu thành công.";
        public const string SaveDataFailed = "Lưu thất bại.";
        public const string Required = "Dữ liệu không được để trống.";
        public const string EmailAddressFormatError = "Email không đúng định dạng.";
        public const string PasswordStringLengthError = "{0} phải có tối thiểu {2} và tối đa {1} kí tự";
        public const string ConfirmPasswordError = "Mật khẩu nhập lại không đúng.";
        public const string FormatDate = "dd/MM/yyyy";
        public const string MaxlengthError = "{0} độ dài tối đa {1} ký tự.";
        public const string APIURL = "http://localhost:5000/";
        public const string POLICY_VERIFY_EMAIL = "VerifyEmail";
        public const string CLAIM_USER_TYPE = "user_type";
        public const string CLAIM_FULL_NAME = "full_name";
        public const string CLAIM_EMAIL = "email";
        public const string CLAIM_ID = "id";
        public const string REFFER_CODE = "refferCode";
        public const string IS_SELLER = "is_seller";
        public const string IS_ADMIN = "is_admin";
        public const string AVATAR = "avatar";
        public const string USERNAME = "username";
        public const string IS_CREATOR = "is_creator";
        public const string IS_ARTIST = "is_artist";
        public const string FormatDateTime = "dd/MM/yyyy HH:mm";
        public const string FormatFullDateTime = "dd/MM/yyyy HH:mm:ss";
        public const string UserNameAdminSeller = "seller";
        public const string UserRole = "user";
        public const string ROLE = "role";
        public const string Email = "email";

        public const string SellerRole = "seller";
        public const string PlacementBuyNow = "buy_now";
        public const string PlacementProceedCheckout = "proceed_to_checkout";
        public const int PAYMENT_COD = 1;
        public const int PAYMENT_MOMO = 2;
        public const string PhoneNumberVietNam = "+84";
        public const string SomeThingWentWrong = "Có lỗi xảy ra trong quá trình thực hiện, vui lòng thử lại sau ít phút!";
        public const string CheckSuccess = "Kiểm tra thành công.";
        public const string CheckFailed = "Kiểm tra thất bại.";

        #endregion

        #region Auth
        public const string UserNotFound = "Người dùng không tồn tại.";
        public const string UserExisted = "Người dùng đã tồn tại.";
        public const string EmailIsExisted = "Email đã tồn tại.";
        public const string EmailIsNotExisted = "Email không tồn tại.";
        public const string EmailIsNotConfirmed = "Email chưa được xác nhận.";
        public const string UserBanned = "User đã bị khóa";

        public const string EmailConfirmed = "Email đã được xác nhận.";
        public const string EmailNotConfirmed = "Email chưa được xác nhận.";

        #endregion


        #region Password
        public const string REGEX_PASSWORD = @"^[A-Za-z0-9!@#?$%^&*()_[\]{}|:"",.<>+=-]*$";
        public const string PasswordInvalidFormat = "Mật khẩu chỉ được chứa ký tự chữ cái, chữ số, ký tự đặc biệt.";
        public const string PasswordIsInCorrect = "Tài khoản hoặc mật khẩu không đúng";
        #endregion

        #region Role
        public const string RoleIsExisted = "Role đã tồn tại.";
        #endregion


        public const string REGEX_LINK_YOUTUBE = @"^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$";
        public const string LinkYoutubeInvalidFormat = "Đường link youtube chưa đúng định dạng.";
        public const string DefaultBatchCode = "BC_0_0";
        public const string StockInApproved = "StockIn Approved";
        public const string StockOutApproved = "StockOut Approved";
        public const string CreatorRoleName = "Creator";
        public const string IllegalAccessToken = "IllegalAccessToken";
        public const string ErrorAuth = "error_auth";
        public const int GHN_NHANH_SERVICE_ID = 1;
        public const int GHN_CHUAN_SERVICE_ID = 2;
        public const int GHN_TIETKIEM_SERVICE_ID = 3;
        public const int Level = 3; //Keyword thấp nhất là cấp 3
        public const int Limit = 1000;
    }
}
