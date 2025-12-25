using ALAP.BLL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models.Wapper;
using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryTestController : BaseAPIController
    {
        private readonly IEntryTestBizLogic _entryTestBizLogic;

        public EntryTestController(IEntryTestBizLogic entryTestBizLogic)
        {
            _entryTestBizLogic = entryTestBizLogic;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEntryTestById(long id)
        {
            try
            {
                var result = await _entryTestBizLogic.GetEntryTestById(id);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("active-list")]
        public async Task<IActionResult> GetAllActiveEntryTests()
        {
            try
            {
                var result = await _entryTestBizLogic.GetAllActiveEntryTests();
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("user-result/{entryTestId}")]
        [Authorize]
        public async Task<IActionResult> GetUserTestResult(long entryTestId)
        {
            try
            {
                var result = await _entryTestBizLogic.GetUserTestResult(UserId, entryTestId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetListEntryTestsByPaging([FromQuery] PagingModel pagingModel)
        {
            try
            {
                var result = await _entryTestBizLogic.GetListEntryTestsByPaging(pagingModel);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdateEntryTest([FromBody] CreateUpdateEntryTestDto dto)
        {
            try
            {
                var result = await _entryTestBizLogic.CreateUpdateEntryTest(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpPost("submit")]
        [Authorize]
        public async Task<IActionResult> SubmitEntryTest([FromBody] SubmitEntryTestDto dto)
        {
            try
            {
                var result = await _entryTestBizLogic.SubmitEntryTest(UserId, dto);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEntryTest(long id)
        {
            try
            {
                var result = await _entryTestBizLogic.DeleteEntryTest(id);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }
    }
}
