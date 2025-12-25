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
    public class UserLessonController : BaseAPIController
    {
        private readonly IUserLessonProgressBizLogic _biz;

        public UserLessonController(IUserLessonProgressBizLogic biz)
        {
            _biz = biz;
        }

        [HttpGet("list/{courseId}/{topicId}")]
        [Authorize]
        public async Task<IActionResult> GetByTopic(long courseId, long topicId)
        {
            try
            {
                var res = await _biz.GetByTopic(UserId, courseId, topicId);
                return GetSuccess(res);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("{courseId}/{topicId}/{lessonId}")]
        [Authorize]
        public async Task<IActionResult> Get(long courseId, long topicId, long lessonId)
        {
            try
            {
                var res = await _biz.Get(UserId, courseId, topicId, lessonId);
                return GetSuccess(res);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("create-update")]
        [Authorize]
        public async Task<IActionResult> CreateUpdate([FromBody] CreateUpdateUserLessonDto dto)
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
