using ALAP.BLL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models.Wapper;
using Base.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginHistoryController : BaseAPIController
    {
        private readonly ILoginHistoryBizLogic _loginHistoryBizLogic;

        public LoginHistoryController(ILoginHistoryBizLogic loginHistoryBizLogic)
        {
            _loginHistoryBizLogic = loginHistoryBizLogic;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoginHistoryById(long id)
        {
            try
            {
                var loginHistory = await _loginHistoryBizLogic.GetLoginHistoryById(id);
                return GetSuccess(loginHistory);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetListLoginHistoryByPaging([FromQuery] PagingModel pagingModel)
        {
            try
            {
                var loginHistories = await _loginHistoryBizLogic.GetListLoginHistoryByPaging(pagingModel);
                return GetSuccess(loginHistories);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-user/{userId}")]
        public async Task<IActionResult> GetLoginHistoryByUserId(long userId, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var loginHistories = await _loginHistoryBizLogic.GetLoginHistoryByUserId(userId, pagingModel);
                return GetSuccess(loginHistories);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdateLoginHistory([FromBody] CreateUpdateLoginHistoryDto dto)
        {
            try
            {
                var result = await _loginHistoryBizLogic.CreateUpdateLoginHistory(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLoginHistory(long id)
        {
            try
            {
                var result = await _loginHistoryBizLogic.DeleteLoginHistory(id);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }
    }
}
