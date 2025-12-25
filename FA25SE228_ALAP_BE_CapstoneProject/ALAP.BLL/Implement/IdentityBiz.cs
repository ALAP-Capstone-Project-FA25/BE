using ALAP.BLL.Interface;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Request.User;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class IdentityBiz : IIdentityBiz
    {
        private readonly IIdentityRepository _iidentityRepository;
        private readonly IMapper _mapper;


        public IdentityBiz(IIdentityRepository iidentityRepository, IMapper mapper)
        {
            _iidentityRepository = iidentityRepository;
            _mapper = mapper;
       
        }

        public async Task<bool> ChangeIsActive(bool status, int id)
        {
            return await _iidentityRepository.ChangeIsActive(status, id);
        }

        public async Task<bool> CheckEmailAlready(string email)
        {
            return await _iidentityRepository.CheckEmailAlready(email);
        }

        public Task<UserModel> CreateUser(UserModel model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserResponseDTO> GetUsersByEmail(string email)
        {
            var model = await _iidentityRepository.GetUsersByEmail(email);
            var dto = _mapper.Map<UserResponseDTO>(model);
            return dto;
        }

        public async Task<UserResponseDTO> GetUsersById(int id)
        {
            var model = await _iidentityRepository.GetUsersById(id);
            return _mapper.Map<UserResponseDTO>(model);
        }

        public async Task<UserTokenResponse> Login(UserLoginRequestDTO dto)
        {
            return await _iidentityRepository.Login(dto);
        }

        public async Task<UserTokenResponse> LoginGoogleAuthenticator(TwoFactorAuthRequest dto)
        {
            return await _iidentityRepository.LoginGoogleAuthenticator(dto);
        }

        Task<UserModel> IIdentityBiz.GetUsersByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ChangePassword(int userId, ChangePasswordDTO dto) { 
            
            var result = await _iidentityRepository.ChangePassword(userId, dto);
            return result;
    
        }


        public Task<GetUserProfile> GetProfileByUser(int userID)
        {
            var result = _iidentityRepository.GetProfileByUser(userID);
            return result;
        }

        public Task<string> SendMailResetPassword(string email)
        {
            var result = _iidentityRepository.SendMailResetPassword(email);
            return result;
        }

        public Task<string> ResetPassword(string token, string password)
        {
            var result = _iidentityRepository.ResetPassword(token, password);
            return result;
        }

        public async Task<string> VerifyEmailTokenAsync(string token)
        {
            var result = await _iidentityRepository.VerifyEmailTokenAsync(token);
            return result;
        }

        public async Task<UserModel> GetInfo(long userId)
        {
            return await _iidentityRepository.GetInfo(userId);
        }

        public async Task<PagedResult<UserModel>> GetUserByPaging(PagingModel model)
        {
            return await _iidentityRepository.GetUserByPaging(model);
        }

        public async Task<UserTokenResponse> LoginEvent(UserLoginRequestDTO dto)
        {
            return await _iidentityRepository.LoginEvent(dto);
        }

        public Task<string> CreateUserGoogle(UserLoginGoogleDTO dto)
        {
            var result = _iidentityRepository.CreateUserGoogle(dto);
            return result;
        }

        public async Task<UserModel> CreateSpeaker(CreateSpeakerRequestDTO dto)
        {
            return await _iidentityRepository.CreateSpeaker(dto);
        }

        public async Task<UserModel> CreateMentor(CreateMentorRequestDTO dto)
        {
            return await _iidentityRepository.CreateMentor(dto);
        }

        public async Task<object> GetSpeakerDetails(long speakerId)
        {
            return await _iidentityRepository.GetSpeakerDetails(speakerId);
        }

        public async Task<UserModel> UpdateProfile(int userId, UpdateProfileDTO dto)
        {
            return await _iidentityRepository.UpdateProfile(userId, dto);
        }
    }
}
