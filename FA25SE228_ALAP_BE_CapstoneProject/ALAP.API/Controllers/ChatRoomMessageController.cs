using ALAP.API.Hubs;
using ALAP.BLL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models.Wapper;
using Base.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRoomMessageController : BaseAPIController
    {
        private readonly IChatRoomMessageBizLogic _biz;
        private readonly IChatRoomBizLogic _chatRoomBizLogic;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatRoomMessageController(IChatRoomMessageBizLogic biz, IHubContext<ChatHub> hubContext, IChatRoomBizLogic chatRoomMessageBizLogic)
        {
            _biz = biz;
            _hubContext = hubContext;
            _chatRoomBizLogic = chatRoomMessageBizLogic;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var result = await _biz.GetChatRoomMessageById(id);
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
                var result = await _biz.GetListChatRoomMessagesByPaging(pagingModel);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("get-by-chatroom/{chatRoomId}")]
        public async Task<IActionResult> GetByChatRoom(long chatRoomId)
        {
            try
            {
                var result = await _biz.GetMessagesByChatRoomId(chatRoomId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdate([FromBody] CreateUpdateChatRoomMessageDto dto)
        {
            try
            {
                var result = await _biz.CreateUpdateChatRoomMessage(dto);

                if (result)
                {
                    var group = dto.ChatRoomId.ToString();
                    var eventName = dto.Id > 0 ? "MessageUpdated" : "ReceiveMessage";
                    await _hubContext.Clients.Group(group).SendAsync(eventName, dto);
                }

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
                var result = await _biz.DeleteChatRoomMessage(id);
                if (result)
                {
                    await _hubContext.Clients.All.SendAsync("MessageDeleted", id);
                }
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }
    }
}
