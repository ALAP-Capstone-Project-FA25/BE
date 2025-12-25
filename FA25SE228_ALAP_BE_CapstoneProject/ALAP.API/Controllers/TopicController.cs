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
    public class TopicController : BaseAPIController
    {
        private readonly ITopicBizLogic _topicBizLogic;
        private readonly ICourseBizLogic _courseBizLogic;

        public TopicController(ITopicBizLogic topicBizLogic, ICourseBizLogic courseBizLogic)
        {
            _topicBizLogic = topicBizLogic;
            _courseBizLogic = courseBizLogic;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopicById(long id)
        {
            try
            {
                var topic = await _topicBizLogic.GetTopicById(id);
                return GetSuccess(topic);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging")]
        [Authorize]
        public async Task<IActionResult> GetListTopicsByPaging([FromQuery] PagingModel pagingModel)
        {
            try
            {
                pagingModel.UserId = UserId;
                var topics = await _topicBizLogic.GetListTopicsByPaging(pagingModel);
                return GetSuccess(topics);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging-by-student")]
        [Authorize]
        public async Task<IActionResult> GetListTopicsByPagingByStudent([FromQuery] PagingModel pagingModel)
        {
            try
            {
                pagingModel.UserId = UserId;
                var topics = await _topicBizLogic.GetListTopicsByPagingByStudent(pagingModel);
                return GetSuccess(topics);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging-for-mentor")]
        [Authorize]
        public async Task<IActionResult> GetListTopicsByPagingForMentor([FromQuery] PagingModel pagingModel, [FromQuery] long studentId)
        {
            try
            {
                pagingModel.UserId = studentId;
                var topics = await _topicBizLogic.GetListTopicsByPagingByStudent(pagingModel);
                return GetSuccess(topics);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging-with-course")]
        public async Task<IActionResult> GetListTopicsByPagingWithCourse([FromQuery] PagingModel pagingModel)
        {
            try
            {
                var topics = await _topicBizLogic.GetListTopicsByPaging(pagingModel);
                var course = await _courseBizLogic.GetCourseById(pagingModel.CourseId);
                var res = new
                {
                    course = course,
                    topics = topics
                };
                return GetSuccess(res);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdateTopic([FromBody] CreateUpdateTopicDto dto)
        {
            try
            {
                var result = await _topicBizLogic.CreateUpdateTopic(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTopic(long id)
        {
            try
            {
                var result = await _topicBizLogic.DeleteTopic(id);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }
    }
}
