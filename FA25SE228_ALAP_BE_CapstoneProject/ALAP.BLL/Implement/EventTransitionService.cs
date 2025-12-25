using ALAP.BLL.Interface;
using ALAP.BLL.Models;
using ALAP.DAL.Database;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Enums;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class EventTransitionService : IEventTransitionService
    {
        private readonly BaseDBContext _dbContext;
        private readonly IBackgroundEmailQueue _emailQueue;
        private readonly ILogger<EventTransitionService> _logger;
        private const int TICKET_PAGE_SIZE = 500; // Số lượng vé xử lý mỗi trang

        public EventTransitionService(
            BaseDBContext dbContext,
            IBackgroundEmailQueue emailQueue,
            ILogger<EventTransitionService> logger)
        {
            _dbContext = dbContext;
            _emailQueue = emailQueue;
            _logger = logger;
        }

        public async Task<int> TransitionIncomingToInProgressAsync(DateTime now, int batchSize, CancellationToken cancellationToken)
        {
            try
            {
                // Lấy các sự kiện cần chuyển trạng thái
                var events = await _dbContext.Events
                    .Where(e => e.Status == EventStatus.IN_COMING && e.StartDate <= now)
                    .OrderBy(e => e.StartDate)
                    .Take(batchSize)
                    .ToListAsync(cancellationToken);

                if (events.Count == 0)
                {
                    _logger.LogDebug("Không có sự kiện nào cần chuyển từ IN_COMING sang IN_PROGRESS");
                    return 0;
                }

                _logger.LogInformation("Tìm thấy {Count} sự kiện cần chuyển từ IN_COMING sang IN_PROGRESS", events.Count);

                // Chuyển trạng thái trong transaction
                using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    foreach (var ev in events)
                    {
                        // Kiểm tra lại trạng thái để đảm bảo idempotent
                        if (ev.Status != EventStatus.IN_COMING)
                            continue;

                        ev.Status = EventStatus.IN_PROGRESS;
                        ev.UpdatedAt = now;
                    }

                    await _dbContext.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    _logger.LogInformation("Đã chuyển trạng thái thành công {Count} sự kiện sang IN_PROGRESS", events.Count);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Lỗi khi chuyển trạng thái sự kiện sang IN_PROGRESS");
                    throw;
                }

                // Gửi email thông báo (ngoài transaction để không ảnh hưởng đến việc chuyển trạng thái)
                foreach (var ev in events)
                {
                    await EnqueueMeetingStartEmailsAsync(ev, cancellationToken);
                    await CreateEventNotificationsAsync(ev, NotificationType.EVENT_STARTED, cancellationToken);
                }

                return events.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong TransitionIncomingToInProgressAsync");
                throw;
            }
        }

        public async Task<int> TransitionInProgressToCompletedAsync(DateTime now, int batchSize, CancellationToken cancellationToken)
        {
            try
            {
                // Lấy các sự kiện cần chuyển trạng thái
                var events = await _dbContext.Events
                    .Where(e => e.Status == EventStatus.IN_PROGRESS && e.EndDate <= now)
                    .OrderBy(e => e.EndDate)
                    .Take(batchSize)
                    .ToListAsync(cancellationToken);

                if (events.Count == 0)
                {
                    _logger.LogDebug("Không có sự kiện nào cần chuyển từ IN_PROGRESS sang COMPLETED");
                    return 0;
                }

                _logger.LogInformation("Tìm thấy {Count} sự kiện cần chuyển từ IN_PROGRESS sang COMPLETED", events.Count);

                // Chuyển trạng thái trong transaction
                using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    foreach (var ev in events)
                    {
                        // Kiểm tra lại trạng thái để đảm bảo idempotent
                        if (ev.Status != EventStatus.IN_PROGRESS)
                            continue;

                        ev.Status = EventStatus.COMPLETED;
                        ev.UpdatedAt = now;
                    }

                    await _dbContext.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    _logger.LogInformation("Đã chuyển trạng thái thành công {Count} sự kiện sang COMPLETED", events.Count);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Lỗi khi chuyển trạng thái sự kiện sang COMPLETED");
                    throw;
                }

                // Gửi email cảm ơn (ngoài transaction)
                foreach (var ev in events)
                {
                    await EnqueueThankYouEmailsAsync(ev, cancellationToken);
                    await CreateEventNotificationsAsync(ev, NotificationType.EVENT_ENDED, cancellationToken);
                }

                return events.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong TransitionInProgressToCompletedAsync");
                throw;
            }
        }

        /// <summary>
        /// Thêm email thông báo sự kiện bắt đầu vào hàng đợi
        /// </summary>
        private async Task EnqueueMeetingStartEmailsAsync(EventModel eventModel, CancellationToken cancellationToken)
        {
            try
            {
                var page = 0;
                var totalEnqueued = 0;

                while (!cancellationToken.IsCancellationRequested)
                {
                    // Lấy vé theo trang để tránh load quá nhiều vào memory
                    var tickets = await _dbContext.EventTickets
                        .AsNoTracking()
                        .Include(t => t.User)
                        .Where(t => t.EventId == eventModel.Id && t.IsActive)
                        .OrderBy(t => t.Id)
                        .Skip(page * TICKET_PAGE_SIZE)
                        .Take(TICKET_PAGE_SIZE)
                        .ToListAsync(cancellationToken);

                    if (tickets.Count == 0)
                        break;

                    foreach (var ticket in tickets)
                    {
                        if (string.IsNullOrEmpty(ticket.User?.Email))
                            continue;

                        var recipientName = !string.IsNullOrEmpty(ticket.User.FirstName) && !string.IsNullOrEmpty(ticket.User.LastName)
                            ? $"{ticket.User.FirstName} {ticket.User.LastName}"
                            : ticket.User.Username;

                        var emailBody = BuildMeetingStartEmailBody(eventModel, recipientName);
                        var emailMessage = new EmailMessage(
                            to: ticket.User.Email,
                            subject: $"[{eventModel.Title}] Sự kiện bắt đầu — Link tham gia",
                            htmlBody: emailBody,
                            recipientName: recipientName
                        );

                        _emailQueue.Enqueue(emailMessage);
                        totalEnqueued++;
                    }

                    page++;
                }

                _logger.LogInformation("Đã thêm {Count} email thông báo bắt đầu cho sự kiện '{Title}' vào hàng đợi",
                    totalEnqueued, eventModel.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm email thông báo bắt đầu cho sự kiện {EventId}", eventModel.Id);
            }
        }

        /// <summary>
        /// Thêm email cảm ơn vào hàng đợi
        /// </summary>
        private async Task EnqueueThankYouEmailsAsync(EventModel eventModel, CancellationToken cancellationToken)
        {
            try
            {
                var page = 0;
                var totalEnqueued = 0;

                while (!cancellationToken.IsCancellationRequested)
                {
                    var tickets = await _dbContext.EventTickets
                        .AsNoTracking()
                        .Include(t => t.User)
                        .Where(t => t.EventId == eventModel.Id && t.IsActive)
                        .OrderBy(t => t.Id)
                        .Skip(page * TICKET_PAGE_SIZE)
                        .Take(TICKET_PAGE_SIZE)
                        .ToListAsync(cancellationToken);

                    if (tickets.Count == 0)
                        break;

                    foreach (var ticket in tickets)
                    {
                        if (string.IsNullOrEmpty(ticket.User?.Email))
                            continue;

                        var recipientName = !string.IsNullOrEmpty(ticket.User.FirstName) && !string.IsNullOrEmpty(ticket.User.LastName)
                            ? $"{ticket.User.FirstName} {ticket.User.LastName}"
                            : ticket.User.Username;

                        var emailBody = BuildThankYouEmailBody(eventModel, recipientName);
                        var emailMessage = new EmailMessage(
                            to: ticket.User.Email,
                            subject: $"[{eventModel.Title}] Cảm ơn bạn đã tham dự",
                            htmlBody: emailBody,
                            recipientName: recipientName
                        );

                        _emailQueue.Enqueue(emailMessage);
                        totalEnqueued++;
                    }

                    page++;
                }

                _logger.LogInformation("Đã thêm {Count} email cảm ơn cho sự kiện '{Title}' vào hàng đợi",
                    totalEnqueued, eventModel.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm email cảm ơn cho sự kiện {EventId}", eventModel.Id);
            }
        }

        /// <summary>
        /// Tạo nội dung email thông báo sự kiện bắt đầu
        /// </summary>
        private string BuildMeetingStartEmailBody(EventModel eventModel, string recipientName)
        {
            var meetingLink = !string.IsNullOrEmpty(eventModel.MeetingLink)
                ? eventModel.MeetingLink
                : "#";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px; margin: 15px 0; }}
        .footer {{ margin-top: 20px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Sự kiện đã bắt đầu!</h1>
        </div>
        <div class='content'>
            <p>Xin chào <strong>{recipientName}</strong>,</p>
            
            <p>Sự kiện <strong>{eventModel.Title}</strong> đã chính thức bắt đầu!</p>
            
            <p><strong>Thời gian bắt đầu:</strong> {eventModel.StartDate:dd/MM/yyyy HH:mm}</p>
            <p><strong>Thời gian kết thúc:</strong> {eventModel.EndDate:dd/MM/yyyy HH:mm}</p>
            
            <p>Vui lòng nhấn vào nút bên dưới để tham gia sự kiện:</p>
            
            <div style='text-align: center;'>
                <a href='{meetingLink}' class='button'>🔗 Tham gia ngay</a>
            </div>
            
            <p>Hoặc sao chép link sau vào trình duyệt:</p>
            <p style='background-color: #fff; padding: 10px; border-left: 3px solid #4CAF50;'>
                <a href='{meetingLink}'>{meetingLink}</a>
            </p>
            
            <p>Chúc bạn có trải nghiệm tuyệt vời!</p>
            
            <div class='footer'>
                <p>Trân trọng,<br>Đội ngũ ALAP</p>
                <p><em>Email này được gửi tự động, vui lòng không trả lời.</em></p>
            </div>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Tạo nội dung email cảm ơn
        /// </summary>
        private string BuildThankYouEmailBody(EventModel eventModel, string recipientName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px; }}
        .footer {{ margin-top: 20px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Cảm ơn bạn đã tham dự!</h1>
        </div>
        <div class='content'>
            <p>Xin chào <strong>{recipientName}</strong>,</p>
            
            <p>Sự kiện <strong>{eventModel.Title}</strong> đã kết thúc thành công!</p>
            
            <p>Chúng tôi xin chân thành cảm ơn bạn đã dành thời gian tham gia và đồng hành cùng chúng tôi trong sự kiện này.</p>
            
            <p>Hy vọng bạn đã có những trải nghiệm bổ ích và thú vị. Chúng tôi rất mong được gặp lại bạn trong các sự kiện sắp tới!</p>
            
            <p>Nếu bạn có bất kỳ góp ý hoặc phản hồi nào, đừng ngần ngại liên hệ với chúng tôi.</p>
            
            <div class='footer'>
                <p>Trân trọng,<br>Đội ngũ ALAP</p>
                <p><em>Email này được gửi tự động, vui lòng không trả lời.</em></p>
            </div>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Tạo notification cho tất cả người dùng có vé sự kiện
        /// </summary>
        private async Task CreateEventNotificationsAsync(EventModel eventModel, NotificationType type, CancellationToken cancellationToken)
        {
            try
            {
                var page = 0;
                var totalCreated = 0;

                while (!cancellationToken.IsCancellationRequested)
                {
                    var tickets = await _dbContext.EventTickets
                        .AsNoTracking()
                        .Where(t => t.EventId == eventModel.Id && t.IsActive)
                        .OrderBy(t => t.Id)
                        .Skip(page * TICKET_PAGE_SIZE)
                        .Take(TICKET_PAGE_SIZE)
                        .ToListAsync(cancellationToken);

                    if (tickets.Count == 0)
                        break;

                    var notifications = new List<NotificationModel>();
                    var now = Utils.GetCurrentVNTime();

                    foreach (var ticket in tickets)
                    {
                        string title, message;
                        switch (type)
                        {
                            case NotificationType.EVENT_STARTED:
                                title = $"Sự kiện {eventModel.Title} đã bắt đầu";
                                message = $"Sự kiện {eventModel.Title} đã chính thức bắt đầu. Vui lòng tham gia ngay!";
                                break;
                            case NotificationType.EVENT_ENDED:
                                title = $"Sự kiện {eventModel.Title} đã kết thúc";
                                message = $"Sự kiện {eventModel.Title} đã kết thúc. Cảm ơn bạn đã tham gia!";
                                break;
                            default:
                                continue;
                        }

                        notifications.Add(new NotificationModel
                        {
                            UserId = ticket.UserId,
                            Type = type,
                            Title = title,
                            Message = message,
                            LinkUrl = $"/my-event-ticket",
                            IsRead = false,
                            CreatedAt = now,
                            UpdatedAt = now
                        });
                    }

                    if (notifications.Any())
                    {
                        await _dbContext.Notifications.AddRangeAsync(notifications, cancellationToken);
                        await _dbContext.SaveChangesAsync(cancellationToken);
                        totalCreated += notifications.Count;
                    }

                    page++;
                }

                _logger.LogInformation("Đã tạo {Count} notification cho sự kiện '{Title}'",
                    totalCreated, eventModel.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo notification cho sự kiện {EventId}", eventModel.Id);
            }
        }

        public async Task<int> CreateUpcomingEventNotificationsAsync(DateTime now, int batchSize, CancellationToken cancellationToken)
        {
            try
            {
                // Lấy các sự kiện sắp đến hạn (1 ngày trước khi bắt đầu)
                var oneDayFromNow = now.AddDays(1);
                var events = await _dbContext.Events
                    .Where(e => e.Status == EventStatus.IN_COMING
                        && e.StartDate > now
                        && e.StartDate <= oneDayFromNow)
                    .OrderBy(e => e.StartDate)
                    .Take(batchSize)
                    .ToListAsync(cancellationToken);

                if (events.Count == 0)
                {
                    _logger.LogDebug("Không có sự kiện nào sắp đến hạn trong 24h tới");
                    return 0;
                }

                _logger.LogInformation("Tìm thấy {Count} sự kiện sắp đến hạn trong 24h tới", events.Count);

                var totalCreated = 0;

                foreach (var ev in events)
                {
                    try
                    {
                        // Check if notification already exists for this event (to avoid duplicates)
                        var existingNotifications = await _dbContext.Notifications
                            .Where(n => n.Type == NotificationType.EVENT_UPCOMING
                                && n.Metadata != null
                                && n.Metadata.Contains($"\"eventId\":{ev.Id}"))
                            .AnyAsync(cancellationToken);

                        if (existingNotifications)
                            continue;

                        // Get all active tickets for this event
                        var tickets = await _dbContext.EventTickets
                            .AsNoTracking()
                            .Where(t => t.EventId == ev.Id && t.IsActive)
                            .ToListAsync(cancellationToken);

                        if (tickets.Count == 0)
                            continue;

                        var notifications = new List<NotificationModel>();
                        var notificationTime = Utils.GetCurrentVNTime();

                        foreach (var ticket in tickets)
                        {
                            notifications.Add(new NotificationModel
                            {
                                UserId = ticket.UserId,
                                Type = NotificationType.EVENT_UPCOMING,
                                Title = $"Sự kiện {ev.Title} sắp bắt đầu",
                                Message = $"Sự kiện {ev.Title} sẽ bắt đầu vào {ev.StartDate:dd/MM/yyyy HH:mm}. Hãy chuẩn bị tham gia!",
                                LinkUrl = "/my-event-ticket",
                                IsRead = false,
                                CreatedAt = notificationTime,
                                UpdatedAt = notificationTime,
                                Metadata = $"{{\"eventId\":{ev.Id},\"eventTitle\":\"{ev.Title}\"}}"
                            });
                        }

                        if (notifications.Any())
                        {
                            await _dbContext.Notifications.AddRangeAsync(notifications, cancellationToken);
                            await _dbContext.SaveChangesAsync(cancellationToken);
                            totalCreated += notifications.Count;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi khi tạo notification sắp đến hạn cho sự kiện {EventId}", ev.Id);
                    }
                }

                if (totalCreated > 0)
                {
                    _logger.LogInformation("Đã tạo {Count} notification sắp đến hạn cho {EventCount} sự kiện",
                        totalCreated, events.Count);
                }

                return totalCreated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong CreateUpcomingEventNotificationsAsync");
                throw;
            }
        }
    }
}

