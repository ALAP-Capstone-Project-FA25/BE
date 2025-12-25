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
    public class TopicQuestionController : BaseAPIController
    {
        private readonly ITopicQuestionBizLogic _biz;

        public TopicQuestionController(ITopicQuestionBizLogic biz)
        {
            _biz = biz;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var item = await _biz.GetById(id);
                return GetSuccess(item);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetListByPaging([FromQuery] PagingModel pagingModel, [FromQuery] long TopicId)
        {
            try
            {
                var items = await _biz.GetListByPaging(pagingModel, TopicId);
                return GetSuccess(items);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdate([FromBody] CreateUpdateTopicQuestionDto dto)
        {
            try
            {
                var result = await _biz.CreateUpdateTopicQuestion(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpPost("create-update-with-answers")]
        public async Task<IActionResult> CreateUpdateWithAnswers([FromBody] CreateUpdateTopicQuestionWithAnswersDto dto)
        {
            try
            {
                var result = await _biz.CreateUpdateTopicQuestionWithAnswers(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await _biz.Delete(id);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }
    }
}
