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
    [Authorize]
    public class TopicQuizController : BaseAPIController
    {
        private readonly ITopicQuizBizLogic _quizBizLogic;

        public TopicQuizController(ITopicQuizBizLogic quizBizLogic)
        {
            _quizBizLogic = quizBizLogic;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] SubmitTopicQuizDto dto)
        {
            try
            {
                var result = await _quizBizLogic.SubmitTopicQuiz(UserId, dto);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("suggested-lessons/{topicId}")]
        public async Task<IActionResult> GetSuggestedLessons([FromRoute] long topicId)
        {
            try
            {
                var lessons = await _quizBizLogic.GetSuggestedLessons(UserId, topicId);
                return GetSuccess(lessons);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }
    }
}
