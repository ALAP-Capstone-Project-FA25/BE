using ALAP.BLL.Helper;
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
    public class CourseController : BaseAPIController
    {
        private readonly ICourseBizLogic _courseBizLogic;
        private readonly PayOsClient _payos;
        public CourseController(ICourseBizLogic courseBizLogic, PayOsClient payos)
        {
            _courseBizLogic = courseBizLogic;
            _payos = payos;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(long id)
        {
            try
            {
                var course = await _courseBizLogic.GetCourseById(id);
                return GetSuccess(course);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("assign-mentor-to-course")]
        public async Task<IActionResult> AssignMentorToCourse([FromBody] AssignMentorToCourse dto)
        {
            try
            {
                var result = await _courseBizLogic.AssignMentorToCourse(dto.CourseId, dto.MentorId);
                return SaveSuccess(result);

            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }


        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetListCoursesByPaging([FromQuery] PagingModel pagingModel)
        {
            try
            {
                var courses = await _courseBizLogic.GetListCoursesByPaging(pagingModel);
                return GetSuccess(courses);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }


        [HttpGet("get-by-paging-by-category/{categoryId}")]
        public async Task<IActionResult> GetListCoursesByPagingByCourseId([FromRoute] long categoryId)
        {
            try
            {
                var courses = await _courseBizLogic.GetListCourseByCategoryId(categoryId);
                return GetSuccess(courses);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }


        [HttpGet("get-by-paging-by-user")]
        [Authorize]
        public async Task<IActionResult> GetListCoursesByUser([FromQuery] PagingModel pagingModel)
        {
            try
            {
                pagingModel.UserId = UserId;
                var courses = await _courseBizLogic.GetListCoursesByPaging(pagingModel);
                return GetSuccess(courses);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }


        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdateCourse([FromBody] CreateUpdateCourseDto dto)
        {
            try
            {
                var result = await _courseBizLogic.CreateUpdateCourse(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentRequest dto)
        {
            try
            {
                var res = await _payos.CreatePaymentAsync(dto);

                return Ok(new
                {
                    res.Code,
                    res.Desc,
                    res.Signature,
                    res.Data?.PaymentLinkId,
                    res.Data?.CheckoutUrl,
                    res.Data?.QrCode,
                    res.Data?.Amount,
                    res.Data?.OrderCode
                });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }




        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCourse(long id)
        {
            try
            {
                var result = await _courseBizLogic.DeleteCourse(id);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }
    }
}
