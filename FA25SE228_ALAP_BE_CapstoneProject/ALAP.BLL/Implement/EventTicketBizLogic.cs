using ALAP.BLL.Helper;
using ALAP.BLL.Interface;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Enums;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class EventTicketBizLogic : AppBaseBizLogic, IEventTicketBizLogic
    {
        private readonly IEventTicketRepository _ticketRepo;
        private readonly IEventRepository _eventRepo;
        private readonly PayOsClient _payos;
        private readonly IEmailBizLogic _emailBizLogic;

        public EventTicketBizLogic(BaseDBContext dbContext, IEventTicketRepository ticketRepo, IEventRepository eventRepo, PayOsClient payos, IEmailBizLogic emailBizLogic)
            : base(dbContext)
        {
            _ticketRepo = ticketRepo;
            _eventRepo = eventRepo;
            _payos = payos;
            _emailBizLogic = emailBizLogic;
        }

        public async Task<bool> CreateUpdateEventTicket(CreateUpdateEventTicketDto dto)
        {
            if (dto.Id > 0)
            {
                var model = new EventTicketModel
                {
                    Id = dto.Id,
                    EventId = dto.EventId,
                    UserId = dto.UserId,
                    Amount = dto.Amount,
                    UpdatedAt = Utils.GetCurrentVNTime(),
                };
                return await _ticketRepo.Update(model);
            }
            else
            {
                var model = new EventTicketModel
                {
                    EventId = dto.EventId,
                    UserId = dto.UserId,
                    Amount = dto.Amount,
                };
                return await _ticketRepo.Create(model);
            }
        }

        public async Task<bool> DeleteEventTicket(long id)
        {
            return await _ticketRepo.Delete(id);
        }

        public async Task<EventTicketModel> GetEventTicketById(long id)
        {
            return await _ticketRepo.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy vé sự kiện.");
        }

        public Task<PagedResult<EventTicketModel>> GetListEventTicketsByPaging(PagingModel pagingModel, long eventId)
        {
            return _ticketRepo.GetListByPaging(pagingModel, eventId);
        }

        public async Task<string> BuyTicket(long eventId, long userId)
        {
            var ev = await _eventRepo.GetById(eventId);
            if (ev == null)
            {
                throw new KeyNotFoundException("Sự kiện không tồn tại.");
            }

            // Check if user already has a ticket for this event
            var existingTicket = await _dbContext.EventTickets
                .Include(t => t.Payment)
                .FirstOrDefaultAsync(t => t.EventId == eventId && t.UserId == userId);

            if (existingTicket != null)
            {
                // Check if payment is successful
                if (existingTicket.Payment.PaymentStatus == PaymentStatus.SUCCESS)
                {
                    throw new InvalidOperationException("Bạn đã mua vé cho sự kiện này rồi.");
                }

                // Check if ticket is still valid (within 1 hour)
                var ticketAge = Utils.GetCurrentVNTime() - existingTicket.CreatedAt;
                if (ticketAge.TotalHours < 1)
                {
                    // Return existing payment URL
                    return existingTicket.Payment.PaymentUrl;
                }
                else
                {
                    // Ticket expired, delete old ticket and create new one
                    _dbContext.EventTickets.Remove(existingTicket);
                    await _dbContext.SaveChangesAsync();
                }
            }

            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var dto = new CreatePaymentRequest
            {
                Amount = (int)ev.Amount,
                Description = "Thanh toan ve su kien",
                OrderCode = orderCode,
                CancelUrl = "https://alap.fptzone.site/my-event-ticket",
                ReturnUrl = "https://alap.fptzone.site/my-event-ticket",
            };

            var res = await _payos.CreatePaymentAsync(dto);
            var item = Utils.SerializeObjectToJson(ev);
            var paymentModel = new PaymentModel
            {
                UserId = userId,
                Item = item,
                Amount = (int)ev.Amount,
                Code = orderCode,
                PaymentUrl = res.Data?.CheckoutUrl ?? "",
                PaymentStatus = PaymentStatus.PENDING,
                PaymentType = PaymentType.EVENT,
                QrCode = res.Data?.QrCode ?? "",
                CreatedAt = Utils.GetCurrentVNTime(),
            };

            await _dbContext.Payments.AddAsync(paymentModel);
            await _dbContext.SaveChangesAsync();

            var ticket = new EventTicketModel
            {
                EventId = eventId,
                UserId = userId,
                Amount = (long)ev.Amount,
                PaymentId = paymentModel.Id,
            };
            await _dbContext.EventTickets.AddAsync(ticket);
            await _dbContext.SaveChangesAsync();

            return res.Data?.CheckoutUrl ?? "";
        }

        public async Task<List<EventTicketModel>> GetMyTickets(long userId)
        {
            return await _dbContext.EventTickets
                .Include(x => x.Event)
                .Include(x => x.Payment)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<PagedResult<EventTicketModel>> GetRefundList(RefundFilterModel filter)
        {
            var query = _dbContext.EventTickets
                .Include(t => t.User)
                .Include(t => t.Event)
                .Include(t => t.Payment)
                .Where(t => t.NeedRefund == true);

            // Filter by EventId
            if (filter.EventId.HasValue)
            {
                query = query.Where(t => t.EventId == filter.EventId.Value);
            }

            // Filter by IsRefunded
            if (filter.IsRefunded.HasValue)
            {
                query = query.Where(t => t.IsRefunded == filter.IsRefunded.Value);
            }

            // Search by keyword (user name or event title)
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword;
                query = query.Where(t =>
                    EF.Functions.Like(t.User.FirstName, $"%{keyword}%") ||
                    EF.Functions.Like(t.User.LastName, $"%{keyword}%") ||
                    EF.Functions.Like(t.Event.Title, $"%{keyword}%")
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<EventTicketModel>(items, totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<bool> UpdateRefundStatus(UpdateRefundDto dto)
        {
            var ticket = await _dbContext.EventTickets
                .Include(t => t.User)
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.Id == dto.TicketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException("Không tìm thấy vé.");
            }

            if (!ticket.NeedRefund)
            {
                throw new InvalidOperationException("Vé này không cần hoàn tiền.");
            }

            // Update refund information
            ticket.RefundImageUrl = dto.RefundImageUrl;
            ticket.IsRefunded = dto.IsRefunded;
            ticket.UpdatedAt = Utils.GetCurrentVNTime();

            await _dbContext.SaveChangesAsync();

            // Send email notification if refund is completed
            if (dto.IsRefunded && ticket.User != null)
            {
                try
                {
                    await SendRefundNotificationEmail(ticket, dto.RefundImageUrl);
                }
                catch (Exception ex)
                {
                    // Log error but don't throw - refund status should still be updated
                    Console.WriteLine($"[Email Error] Failed to send refund notification: {ex.Message}");
                }

                // Create notification for refund
                try
                {
                    var notification = new NotificationModel
                    {
                        UserId = ticket.UserId,
                        Type = NotificationType.REFUND_PROCESSED,
                        Title = $"Hoàn tiền cho sự kiện {ticket.Event?.Title}",
                        Message = $"Bạn đã được hoàn tiền {ticket.Amount:N0} VNĐ cho sự kiện {ticket.Event?.Title}.",
                        LinkUrl = "/my-event-ticket",
                        IsRead = false,
                        CreatedAt = Utils.GetCurrentVNTime(),
                        UpdatedAt = Utils.GetCurrentVNTime()
                    };

                    await _dbContext.Notifications.AddAsync(notification);
                    await _dbContext.SaveChangesAsync();
                }
                catch
                {
                    // Silently fail - notification is not critical
                }
            }

            return true;
        }

        public async Task<RefundStatisticsDto> GetRefundStatistics(long eventId)
        {
            var tickets = await _dbContext.EventTickets
                .Where(t => t.EventId == eventId && t.NeedRefund == true)
                .ToListAsync();

            var refundedTickets = tickets.Where(t => t.IsRefunded).ToList();

            return new RefundStatisticsDto
            {
                TotalTicketsNeedRefund = tickets.Count,
                TicketsRefunded = refundedTickets.Count,
                TicketsPendingRefund = tickets.Count - refundedTickets.Count,
                TotalAmountNeedRefund = tickets.Sum(t => t.Amount),
                TotalAmountRefunded = refundedTickets.Sum(t => t.Amount)
            };
        }

        private async Task SendRefundNotificationEmail(EventTicketModel ticket, string refundImageUrl)
        {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "refund-notification.html");
            var emailBody = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders
            emailBody = emailBody.Replace("{{UserName}}", $"{ticket.User.FirstName} {ticket.User.LastName}");
            emailBody = emailBody.Replace("{{EventTitle}}", ticket.Event.Title);
            emailBody = emailBody.Replace("{{Amount}}", ticket.Amount.ToString("N0"));
            emailBody = emailBody.Replace("{{RefundImageUrl}}", refundImageUrl ?? "");

            await _emailBizLogic.SendEmailAsync(
                ticket.User.Email,
                "Thông báo hoàn tiền sự kiện",
                emailBody,
                true
            );
        }

        public async Task<UserTicketStatusDto> CheckUserTicket(long eventId, long userId)
        {
            var ticket = await _dbContext.EventTickets
                .Include(t => t.Payment)
                .FirstOrDefaultAsync(t => t.EventId == eventId && t.UserId == userId);

            if (ticket == null)
            {
                return new UserTicketStatusDto
                {
                    HasTicket = false
                };
            }

            // Check if ticket is expired (1 hour from creation)
            var ticketAge = Utils.GetCurrentVNTime() - ticket.CreatedAt;
            var isExpired = ticketAge.TotalHours >= 1 && ticket.Payment.PaymentStatus != PaymentStatus.SUCCESS;
            var minutesRemaining = isExpired ? 0 : (int)(60 - ticketAge.TotalMinutes);

            return new UserTicketStatusDto
            {
                HasTicket = true,
                TicketId = ticket.Id,
                PaymentId = ticket.PaymentId,
                PaymentStatus = ticket.Payment.PaymentStatus,
                PaymentUrl = ticket.Payment.PaymentUrl,
                Amount = ticket.Amount,
                CreatedAt = ticket.CreatedAt,
                IsExpired = isExpired,
                MinutesRemaining = minutesRemaining > 0 ? minutesRemaining : 0
            };
        }

        public async Task<RefundStatisticsDto> GetRefundStatisticsOverall()
        {
            var tickets = await _dbContext.EventTickets
                .Where(t => t.NeedRefund == true)
                .ToListAsync();

            var refundedTickets = tickets.Where(t => t.IsRefunded).ToList();
            var pendingTickets = tickets.Where(t => !t.IsRefunded).ToList();

            return new RefundStatisticsDto
            {
                TotalTicketsNeedRefund = tickets.Count,
                TicketsRefunded = refundedTickets.Count,
                TicketsPendingRefund = pendingTickets.Count,
                TotalAmountNeedRefund = tickets.Sum(t => t.Amount),
                TotalAmountRefunded = refundedTickets.Sum(t => t.Amount),
                TotalAmountPendingRefund = pendingTickets.Sum(t => t.Amount)
            };
        }
    }
}

