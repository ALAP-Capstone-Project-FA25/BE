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
    public class MajorController : BaseAPIController
    {
        private readonly IMajorBizLogic _majorBizLogic;

        public MajorController(IMajorBizLogic majorBizLogic)
        {
            _majorBizLogic = majorBizLogic;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMajorById(long id)
        {
            try
            {
                var result = await _majorBizLogic.GetMajorById(id);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetListMajorsByPaging([FromQuery] PagingModel pagingModel)
        {
            try
            {
                var result = await _majorBizLogic.GetListMajorsByPaging(pagingModel);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdateMajor([FromBody] CreateUpdateMajorDto dto)
        {
            try
            {
                var result = await _majorBizLogic.CreateUpdateMajor(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpPost("update-user-major/{majorId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserMajor([FromRoute] long majorId)
        {
            try
            {
                var result = await _majorBizLogic.UpdateUserMajor(UserId, majorId);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMajor(long id)
        {
            try
            {
                var result = await _majorBizLogic.DeleteMajor(id);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }
    }
}
