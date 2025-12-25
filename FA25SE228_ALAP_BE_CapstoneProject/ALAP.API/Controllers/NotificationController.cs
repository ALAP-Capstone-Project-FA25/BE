using ALAP.BLL.Interface;
using ALAP.Entity.Models.Wapper;
using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : BaseAPIController
    {
        private readonly INotificationBizLogic _notificationBizLogic;

        public NotificationController(INotificationBizLogic notificationBizLogic)
        {
            _notificationBizLogic = notificationBizLogic;
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetNotificationsByPaging(
            [FromQuery] PagingModel pagingModel,
            [FromQuery] bool? isRead = null
        )
        {
            try
            {
                var result = await _notificationBizLogic.GetNotificationsByUserId(
                    (long)UserId,
                    pagingModel,
                    isRead
                );
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var count = await _notificationBizLogic.GetUnreadCount((long)UserId);
                return GetSuccess(new { count });
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPost("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(long id)
        {
            try
            {
                var result = await _notificationBizLogic.MarkAsRead(id, (long)UserId);
                if (result)
                    return SaveSuccess(result);
                return GetError("Notification not found");
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpPost("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var result = await _notificationBizLogic.MarkAllAsRead((long)UserId);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(long id)
        {
            try
            {
                var result = await _notificationBizLogic.DeleteNotification(id, (long)UserId);
                if (result)
                    return SaveSuccess(result);
                return GetError("Notification not found");
            }
            catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }
    }
}
