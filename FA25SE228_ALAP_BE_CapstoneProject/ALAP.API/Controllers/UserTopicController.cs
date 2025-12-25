using ALAP.BLL.Interface;
using ALAP.Entity.DTO.Request;
using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTopicController : BaseAPIController
    {
        private readonly IUserTopicProgressBizLogic _biz;

        public UserTopicController(IUserTopicProgressBizLogic biz)
        {
            _biz = biz;
        }

        [HttpGet("list/{courseId}")]
        [Authorize]
        public async Task<IActionResult> GetByCourse(long courseId)
        {
            try
            {
                var res = await _biz.GetByCourse(UserId, courseId);
                return GetSuccess(res);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("{courseId}/{topicId}")]
        [Authorize]
        public async Task<IActionResult> Get(long courseId, long topicId)
        {
            try
            {
                var res = await _biz.Get(UserId, courseId, topicId);
                return GetSuccess(res);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("create-update")]
        [Authorize]
        public async Task<IActionResult> CreateUpdate([FromBody] CreateUpdateUserTopicDto dto)
        {
            try
            {
                var res = await _biz.CreateOrUpdate(UserId, dto);
                return SaveSuccess(res);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }
    }
}
