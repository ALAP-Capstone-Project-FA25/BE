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
    public class UserCourseController : BaseAPIController
    {
        private readonly IUserCourseProgressBizLogic _biz;

        public UserCourseController(IUserCourseProgressBizLogic biz)
        {
            _biz = biz;
        }

        [HttpPost("enroll")]
        [Authorize]
        public async Task<IActionResult> Enroll([FromBody] EnrollCourseDto dto)
        {
            try
            {
                var res = await _biz.Enroll(UserId, dto.CourseId);
                return SaveSuccess(res);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMy()
        {
            try
            {
                var res = await _biz.GetMyCourses(UserId);
                return GetSuccess(res);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-user-course-by-course/{courseId}")]
        public async Task<IActionResult> GetListUserCourseByCourseId(long courseId)
        {
            try
            {
                var res = await _biz.GetListUserCourseByCourseId(courseId);
                return GetSuccess(res);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("{courseId}")]
        [Authorize]
        public async Task<IActionResult> Get(long courseId)
        {
            try
            {
                var res = await _biz.Get(UserId, courseId);
                return GetSuccess(res);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateUserCourseDto dto)
        {
            try
            {
                var res = await _biz.Update(UserId, dto);
                return SaveSuccess(res);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpPut("update-lesson-progress/{topicId}/{lessonId}")]
        public async Task<IActionResult> UpdateLessonProgress(long topicId, long lessonId)
        {
            try
            {
                var res = await _biz.UpdateLessonProgress(topicId, lessonId, UserId);
                return SaveSuccess(res);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpGet("student-progress/{userId}/{courseId}")]
        public async Task<IActionResult> GetStudentProgress(long userId, long courseId)
        {
            try
            {
                var res = await _biz.GetStudentProgress(userId, courseId);
                return GetSuccess(res);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("current-lesson/{userId}/{courseId}")]
        public async Task<IActionResult> GetCurrentLesson(long userId, long courseId)
        {
            try
            {
                var res = await _biz.GetCurrentLesson(userId, courseId);
                return GetSuccess(res);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }
    }
}
