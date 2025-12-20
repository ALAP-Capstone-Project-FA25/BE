using ALAP.BLL.Interface;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class LoginHistoryBizLogic : ILoginHistoryBizLogic
    {
        private readonly ILoginHistoryRepository _loginHistoryRepository;

        public LoginHistoryBizLogic(ILoginHistoryRepository loginHistoryRepository)
        {
            _loginHistoryRepository = loginHistoryRepository;
        }

        public async Task<bool> CreateUpdateLoginHistory(CreateUpdateLoginHistoryDto dto)
        {
            if (dto.Id > 0)
            {
                var loginHistoryModel = new LoginHistoryModel
                {
                    Id = dto.Id,
                    UserId = dto.UserId,
                    LoginDate = dto.LoginDate,
                    IpAddress = dto.IpAddress,
                    UserAgent = dto.UserAgent,
                    UpdatedAt = Utils.GetCurrentVNTime(),
                };
                return await _loginHistoryRepository.Update(loginHistoryModel);
            }
            else
            {
                var loginHistoryModel = new LoginHistoryModel
                {
                    UserId = dto.UserId,
                    LoginDate = dto.LoginDate != default ? dto.LoginDate : Utils.GetCurrentVNTime(),
                    IpAddress = dto.IpAddress,
                    UserAgent = dto.UserAgent,
                };
                return await _loginHistoryRepository.Create(loginHistoryModel);
            }
        }

        public async Task<bool> DeleteLoginHistory(long id)
        {
            return await _loginHistoryRepository.Delete(id);
        }

        public async Task<LoginHistoryModel> GetLoginHistoryById(long id)
        {
            return await _loginHistoryRepository.GetById(id) ?? throw new KeyNotFoundException($"Không tìm thấy lịch sử đăng nhập.");
        }

        public async Task<PagedResult<LoginHistoryModel>> GetListLoginHistoryByPaging(PagingModel pagingModel)
        {
            return await _loginHistoryRepository.GetListByPaging(pagingModel);
        }

        public async Task<PagedResult<LoginHistoryModel>> GetLoginHistoryByUserId(long userId, PagingModel pagingModel)
        {
            return await _loginHistoryRepository.GetByUserId(userId, pagingModel);
        }
    }
}

