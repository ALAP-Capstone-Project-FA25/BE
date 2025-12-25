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
    public class LessonController : BaseAPIController
    {
        private readonly ILessonBizLogic _lessonBizLogic;

        public LessonController(ILessonBizLogic lessonBizLogic)
        {
            _lessonBizLogic = lessonBizLogic;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLessonById(long id)
        {
            try
            {
                var lesson = await _lessonBizLogic.GetLessonById(id);
                return GetSuccess(lesson);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetListLessonsByPaging([FromQuery] PagingModel pagingModel, [FromQuery] long topicId)
        {
            try
            {
                var lessons = await _lessonBizLogic.GetListLessonsByPaging(pagingModel, topicId);
                return GetSuccess(lessons);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdateLesson([FromBody] CreateUpdateLessonDto dto)
        {
            try
            {
                var result = await _lessonBizLogic.CreateUpdateLesson(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLesson(long id)
        {
            try
            {
                var result = await _lessonBizLogic.DeleteLesson(id);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }
    }
}
