using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models.Enums;
using ALAP.Entity.Models.Wapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface INotificationBizLogic
    {
        Task<NotificationDto> CreateNotification(
            long userId,
            NotificationType type,
            string title,
            string? message = null,
            string? linkUrl = null,
            string? metadata = null
        );

        Task<PagedResult<NotificationDto>> GetNotificationsByUserId(
            long userId,
            PagingModel pagingModel,
            bool? isRead = null
        );

        Task<int> GetUnreadCount(long userId);

        Task<bool> MarkAsRead(long notificationId, long userId);

        Task<bool> MarkAllAsRead(long userId);

        Task<bool> DeleteNotification(long notificationId, long userId);
    }
}
