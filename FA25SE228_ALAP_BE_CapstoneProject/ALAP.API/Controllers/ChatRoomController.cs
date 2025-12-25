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
    public class ChatRoomController : BaseAPIController
    {
        private readonly IChatRoomBizLogic _biz;

        public ChatRoomController(IChatRoomBizLogic biz)
        {
            _biz = biz;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var result = await _biz.GetChatRoomById(id);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetListByPaging([FromQuery] PagingModel pagingModel)
        {
            try
            {
                var result = await _biz.GetListChatRoomsByPaging(pagingModel);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }



        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdate([FromBody] CreateUpdateChatRoomDto dto)
        {
            try
            {
                var result = await _biz.CreateUpdateChatRoom(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpGet("get-chat-room-by-course-and-user-id")]
        [Authorize]
        public async Task<IActionResult> GetChatRoomByCourseAndUserId([FromQuery] long courseId)
        {
            try
            {
                var result = await _biz.GetChatRoomByCourseId(courseId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-list-by-mentor-id")]
        public async Task<IActionResult> GetListChatRoomByMentorId()
        {
            try
            {
                var result = await _biz.GetListChatRoomByMentorId(UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await _biz.DeleteChatRoom(id);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }
    }
}
