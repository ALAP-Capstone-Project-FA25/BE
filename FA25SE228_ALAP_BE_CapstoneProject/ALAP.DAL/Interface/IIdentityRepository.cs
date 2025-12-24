using App.Entity.DTO.Request;
using App.Entity.DTO.Request.User;
using App.Entity.DTO.Response;
using App.Entity.Models;
using App.Entity.Models.Wapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DAL.Interface
{
    public interface IIdentityRepository
    {
        Task<UserTokenResponse> Login(UserLoginRequestDTO dto);
        Task<UserModel> CreateUser(UserModel model);
        Task<UserModel> GetUsersById(int id);

        Task<UserModel> GetUsersByEmail(string email);

        Task<bool> DeleteById(int id);

        Task<bool> ChangeIsActive(bool status, int id);

        Task<UserTokenResponse> LoginGoogleAuthenticator(TwoFactorAuthRequest dto);

        Task<long> Register(RegisterDTO request);

        Task<bool> CheckEmailAlready(string email);

        Task<string> VerifyEmailTokenAsync(string token);

        Task<string> ChangePassword(int userID, ChangePasswordDTO request);

        Task<LoginResponse> Login(LoginDTO request);

        Task<GetUserProfile> GetProfileByUser(int userID);

        Task<string> SendMailResetPassword(string email);

        Task<string> ResetPassword(string token, string password);

        Task<UserModel> GetInfo(long userId);

        Task<PagedResult<UserModel>> GetUserByPaging(PagingModel model);

        Task<UserTokenResponse> LoginEvent(UserLoginRequestDTO dto);

        Task<string> CreateUserGoogle(UserLoginGoogleDTO dto);
        Task<UserModel> CreateSpeaker(CreateSpeakerRequestDTO dto);
        Task<UserModel> CreateMentor(CreateMentorRequestDTO dto);
        Task<object> GetSpeakerDetails(long speakerId);
        Task<UserModel> UpdateProfile(int userId, UpdateProfileDTO dto);

    }
}
