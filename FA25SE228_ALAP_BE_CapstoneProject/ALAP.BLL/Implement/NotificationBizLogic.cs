using ALAP.BLL;
using ALAP.BLL.Interface;
using ALAP.DAL.Database;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Enums;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class NotificationBizLogic : AppBaseBizLogic, INotificationBizLogic
    {
        public NotificationBizLogic(BaseDBContext dbContext) : base(dbContext)
        {
        }

        public async Task<NotificationDto> CreateNotification(
            long userId,
            NotificationType type,
            string title,
            string? message = null,
            string? linkUrl = null,
            string? metadata = null
        )
        {
            var notification = new NotificationModel
            {
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                LinkUrl = linkUrl,
                Metadata = metadata,
                IsRead = false,
                CreatedAt = Utils.GetCurrentVNTime(),
                UpdatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Notifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();

            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                LinkUrl = notification.LinkUrl,
                IsRead = notification.IsRead,
                ReadAt = notification.ReadAt,
                CreatedAt = notification.CreatedAt,
                Metadata = notification.Metadata
            };
        }

        public async Task<PagedResult<NotificationDto>> GetNotificationsByUserId(
            long userId,
            PagingModel pagingModel,
            bool? isRead = null
        )
        {
            var query = _dbContext.Notifications
                .Where(n => n.UserId == userId);

            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }

            var totalCount = await query.CountAsync();

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pagingModel.PageNumber - 1) * pagingModel.PageSize)
                .Take(pagingModel.PageSize)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Type = n.Type,
                    Title = n.Title,
                    Message = n.Message,
                    LinkUrl = n.LinkUrl,
                    IsRead = n.IsRead,
                    ReadAt = n.ReadAt,
                    CreatedAt = n.CreatedAt,
                    Metadata = n.Metadata
                })
                .ToListAsync();

            return new PagedResult<NotificationDto>(
                notifications,
                totalCount,
                pagingModel.PageNumber,
                pagingModel.PageSize
            );
        }

        public async Task<int> GetUnreadCount(long userId)
        {
            return await _dbContext.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task<bool> MarkAsRead(long notificationId, long userId)
        {
            var notification = await _dbContext.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return false;

            notification.IsRead = true;
            notification.ReadAt = Utils.GetCurrentVNTime();
            notification.UpdatedAt = Utils.GetCurrentVNTime();

            _dbContext.Notifications.Update(notification);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkAllAsRead(long userId)
        {
            var notifications = await _dbContext.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            var now = Utils.GetCurrentVNTime();
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = now;
                notification.UpdatedAt = now;
            }

            if (notifications.Any())
            {
                _dbContext.Notifications.UpdateRange(notifications);
                await _dbContext.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeleteNotification(long notificationId, long userId)
        {
            var notification = await _dbContext.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return false;

            _dbContext.Notifications.Remove(notification);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
