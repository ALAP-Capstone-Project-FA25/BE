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
    public class EventBizLogic : AppBaseBizLogic, IEventBizLogic
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEmailBizLogic _emailBizLogic;

        public EventBizLogic(BaseDBContext dbContext, IEventRepository eventRepository, IEmailBizLogic emailBizLogic) : base(dbContext)
        {
            _eventRepository = eventRepository;
            _emailBizLogic = emailBizLogic;
        }

        public async Task<bool> CreateUpdateEvent(CreateUpdateEventDto dto)
        {
            if (dto.Id > 0)
            {
                var model = new EventModel
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    StartDate = Utils.ConvertToVietnamTime(dto.StartDate),
                    EndDate = Utils.ConvertToVietnamTime(dto.EndDate),
                    MeetingLink = dto.MeetingLink,
                    CommissionRate = dto.CommissionRate,
                    ImageUrls = dto.ImageUrls,
                    VideoUrl = dto.VideoUrl,
                    Amount = dto.Amount,
                    Status = dto.Status,
                    SpeakerId = dto.SpeakerId,
                    UpdatedAt = Utils.GetCurrentVNTime(),
                };
                return await _eventRepository.Update(model);
            }
            else
            {
                var model = new EventModel
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    VideoUrl = dto.VideoUrl,
                    MeetingLink = dto.MeetingLink,
                    CommissionRate = dto.CommissionRate,
                    ImageUrls = dto.ImageUrls,
                    Amount = dto.Amount,
                    Status = dto.Status,
                    SpeakerId = dto.SpeakerId,
                };
                return await _eventRepository.Create(model);
            }
        }

        public async Task<bool> DeleteEvent(long id)
        {
            return await _eventRepository.Delete(id);
        }

        public async Task<EventModel> GetEventById(long id)
        {
            return await _eventRepository.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy sự kiện.");
        }

        public Task<PagedResult<EventModel>> GetListEventsByPaging(PagingModel pagingModel)
        {
            return _eventRepository.GetListByPaging(pagingModel);
        }

        public async Task<bool> SendCommissionToSpeaker(long eventId, string paymentProofImageUrl = "")
        {
            var existingEvent = await _dbContext.Events
                .Include(e => e.Speaker)
                .FirstOrDefaultAsync(e => e.Id == eventId);
            if (existingEvent == null)
            {
                throw new KeyNotFoundException("Không tìm thấy sự kiện.");
            }
            
            if (existingEvent.IsPaidForSpeaker)
            {
                throw new InvalidOperationException("Đã chuyển tiền cho diễn giả rồi.");
            }
            
            var commission = existingEvent.CommissionRate;
            var totalRevenue = await _dbContext.EventTickets
                .Include(x => x.Payment)
                .Where(et => et.EventId == eventId && et.Payment.PaymentStatus == Entity.Models.Enums.PaymentStatus.SUCCESS)
                .SumAsync(et => et.Amount);

            var commissionAmount = (totalRevenue * commission) / 100;

            existingEvent.Speaker.Balance += commissionAmount;
            existingEvent.IsPaidForSpeaker = true;
            existingEvent.PaymentProofImageUrl = paymentProofImageUrl;
            existingEvent.UpdatedAt = Utils.GetCurrentVNTime();
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<CancelEventResultDto> CancelEvent(long eventId)
        {
            var existingEvent = await _dbContext.Events
                .FirstOrDefaultAsync(e => e.Id == eventId);
            
            if (existingEvent == null)
            {
                throw new KeyNotFoundException("Không tìm thấy sự kiện.");
            }

            // Update event status to CANCELLED
            existingEvent.Status = EventStatus.CANCELLED;
            existingEvent.UpdatedAt = Utils.GetCurrentVNTime();

            // Get all tickets with successful payment (include User for email)
            var ticketsToRefund = await _dbContext.EventTickets
                .Include(t => t.Payment)
                .Include(t => t.User)
                .Where(t => t.EventId == eventId && t.Payment.PaymentStatus == PaymentStatus.SUCCESS)
                .ToListAsync();

            // Mark tickets for refund
            foreach (var ticket in ticketsToRefund)
            {
                ticket.NeedRefund = true;
                ticket.UpdatedAt = Utils.GetCurrentVNTime();
            }

            await _dbContext.SaveChangesAsync();

            // Send cancellation notification emails to all affected users
            foreach (var ticket in ticketsToRefund)
            {
                try
                {
                    await SendEventCancelledNotificationEmail(ticket, existingEvent);
                }
                catch (Exception ex)
                {
                    // Log error but don't throw - continue processing other emails
                    Console.WriteLine($"[Email Error] Failed to send cancellation notification to user {ticket.UserId}: {ex.Message}");
                }
            }

            return new CancelEventResultDto
            {
                Success = true,
                TicketsMarkedForRefund = ticketsToRefund.Count,
                Message = $"Đã hủy sự kiện và đánh dấu {ticketsToRefund.Count} vé cần hoàn tiền."
            };
        }

        private async Task SendEventCancelledNotificationEmail(EventTicketModel ticket, EventModel eventModel)
        {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "event-cancelled-notification.html");
            var emailBody = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders
            emailBody = emailBody.Replace("{{UserName}}", $"{ticket.User.FirstName} {ticket.User.LastName}");
            emailBody = emailBody.Replace("{{EventTitle}}", eventModel.Title);
            emailBody = emailBody.Replace("{{Amount}}", ticket.Amount.ToString("N0"));

            await _emailBizLogic.SendEmailAsync(
                ticket.User.Email,
                $"Thông báo hủy sự kiện: {eventModel.Title}",
                emailBody,
                true
            );
        }
    }
}

