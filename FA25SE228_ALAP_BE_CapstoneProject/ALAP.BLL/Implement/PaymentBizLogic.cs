using ALAP.BLL.Interface;
using ALAP.DAL;
using ALAP.DAL.DataBase;
using ALAP.DAL.Interface;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Enums;
using ALAP.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class PaymentBizLogic : AppBaseBizLogic, IPaymentBizLogic
    {
        private readonly BaseDBContext _dbContext;
        public PaymentBizLogic(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaymentModel> GetPaymentByOrderCode(long orderCode)
        {
            return await _dbContext.Payments
                .Where(p => p.Code == orderCode)
                .FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Payment not found.");
        }

        public async Task<PagedResult<PaymentModel>> GetPaymentByPaging(PagingModel model, PaymentStatus? status = null)
        {
            var query = _dbContext.Payments
                .Include(x => x.Package)
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrEmpty(model.Keyword))
            {
                query = query.Where(x => x.Code.ToString().Contains(model.Keyword));
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.PaymentStatus == status.Value);
            }

            return await query.ToPagedResultAsync(model);
        }

        public async Task<PaymentModel> UpdatePaymentByOrderCode(long orderCode, PaymentStatus status)
        {
            var existingPayment = await _dbContext.Payments
                .Include(p => p.Package)
                .Where(p => p.Code == orderCode)
                .FirstOrDefaultAsync();
            if (existingPayment == null)
            {
                throw new KeyNotFoundException("Payment not found.");
            }
            existingPayment.PaymentStatus = status;

            _dbContext.Payments.Update(existingPayment);

            if (status == PaymentStatus.SUCCESS)
            {
                await ProcessPayment(existingPayment);
            }

            await _dbContext.SaveChangesAsync();
            return existingPayment;
        }


        private async Task ProcessPayment(PaymentModel payment)
        {
            switch (payment.PaymentType)
            {
                case PaymentType.PACKAGE:
                    // Check if user package already exists to avoid duplicate
                    var existingUserPackage = await _dbContext.UserPackages
                        .FirstOrDefaultAsync(up => up.PaymentId == payment.Id);

                    if (existingUserPackage == null)
                    {
                        var newUserPackage = new UserPackageModel
                        {
                            UserId = payment.UserId,
                            PackageId = payment.PackageId.Value,
                            PaymentId = payment.Id,
                            ExpiredAt = DateTime.UtcNow.AddDays(payment.Package!.Duration),
                            IsActive = true
                        };
                        await _dbContext.UserPackages.AddAsync(newUserPackage);
                        await _dbContext.SaveChangesAsync();
                    }

                    // Create notification for package purchase success
                    await CreateNotificationForPayment(payment, NotificationType.PACKAGE_PURCHASE_SUCCESS,
                        $"Mua gói {payment.Package?.Title} thành công",
                        $"Bạn đã mua gói {payment.Package?.Title} thành công. Gói sẽ hết hạn sau {payment.Package?.Duration} ngày.",
                        $"/pricing");
                    break;

                case PaymentType.COURSE:
                    // TODO: Implement course enrollment logic
                    break;

                case PaymentType.EVENT:
                    // Activate event ticket when payment is successful
                    var eventTicket = await _dbContext.EventTickets
                        .Include(et => et.Event)
                        .FirstOrDefaultAsync(et => et.PaymentId == payment.Id);

                    if (eventTicket != null && !eventTicket.IsActive)
                    {
                        eventTicket.IsActive = true;
                        eventTicket.UpdatedAt = Base.Common.Utils.GetCurrentVNTime();
                        _dbContext.EventTickets.Update(eventTicket);
                        await _dbContext.SaveChangesAsync();
                    }

                    // Create notification for event ticket purchase success
                    if (eventTicket != null)
                    {
                        await CreateNotificationForPayment(payment, NotificationType.EVENT_TICKET_PURCHASE_SUCCESS,
                            $"Mua vé sự kiện {eventTicket.Event?.Title} thành công",
                            $"Bạn đã mua vé sự kiện {eventTicket.Event?.Title} thành công. Vui lòng kiểm tra email để xem chi tiết.",
                            $"/my-event-ticket");
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task CreateNotificationForPayment(PaymentModel payment, NotificationType type, string title, string message, string linkUrl)
        {
            var notification = new NotificationModel
            {
                UserId = payment.UserId,
                Type = type,
                Title = title,
                Message = message,
                LinkUrl = linkUrl,
                IsRead = false,
                CreatedAt = Base.Common.Utils.GetCurrentVNTime(),
                UpdatedAt = Base.Common.Utils.GetCurrentVNTime()
            };

            await _dbContext.Notifications.AddAsync(notification);
        }

        public async Task<PaymentModel> UpdatePaymentStatus(long id, PaymentStatus status)
        {
            var existingPayment = await _dbContext.Payments
                .Include(p => p.Package)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (existingPayment == null)
            {
                throw new KeyNotFoundException("Payment not found.");
            }

            existingPayment.PaymentStatus = status;
            _dbContext.Payments.Update(existingPayment);

            if (status == PaymentStatus.SUCCESS)
            {
                await ProcessPayment(existingPayment);
            }

            await _dbContext.SaveChangesAsync();
            return existingPayment;
        }

        public async Task<object> GetPaymentStatistics()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfYear = new DateTime(now.Year, 1, 1);

            var allPayments = await _dbContext.Payments.ToListAsync();
            var successPayments = allPayments.Where(p => p.PaymentStatus == PaymentStatus.SUCCESS).ToList();

            // Tổng doanh thu
            var totalRevenue = successPayments.Sum(p => p.Amount);
            var monthRevenue = successPayments.Where(p => p.CreatedAt >= startOfMonth).Sum(p => p.Amount);
            var yearRevenue = successPayments.Where(p => p.CreatedAt >= startOfYear).Sum(p => p.Amount);

            // Thống kê theo trạng thái
            var statusStats = allPayments
                .GroupBy(p => p.PaymentStatus)
                .Select(g => new
                {
                    Status = (int)g.Key,
                    Count = g.Count(),
                    Amount = g.Sum(p => p.Amount)
                })
                .ToList();

            // Doanh thu theo tháng (12 tháng gần nhất)
            var monthlyRevenue = Enumerable.Range(0, 12)
                .Select(i =>
                {
                    var month = now.AddMonths(-i);
                    var startDate = new DateTime(month.Year, month.Month, 1);
                    var endDate = startDate.AddMonths(1);

                    return new
                    {
                        Month = month.ToString("MM/yyyy"),
                        Revenue = successPayments
                            .Where(p => p.CreatedAt >= startDate && p.CreatedAt < endDate)
                            .Sum(p => p.Amount),
                        Count = successPayments
                            .Where(p => p.CreatedAt >= startDate && p.CreatedAt < endDate)
                            .Count()
                    };
                })
                .OrderBy(x => x.Month)
                .ToList();

            // Doanh thu theo ngày (30 ngày gần nhất)
            var dailyRevenue = Enumerable.Range(0, 30)
                .Select(i =>
                {
                    var date = now.Date.AddDays(-i);
                    var nextDate = date.AddDays(1);

                    return new
                    {
                        Date = date.ToString("dd/MM"),
                        Revenue = successPayments
                            .Where(p => p.CreatedAt >= date && p.CreatedAt < nextDate)
                            .Sum(p => p.Amount),
                        Count = successPayments
                            .Where(p => p.CreatedAt >= date && p.CreatedAt < nextDate)
                            .Count()
                    };
                })
                .OrderBy(x => x.Date)
                .ToList();

            return new
            {
                TotalRevenue = totalRevenue,
                MonthRevenue = monthRevenue,
                YearRevenue = yearRevenue,
                TotalPayments = allPayments.Count,
                SuccessPayments = successPayments.Count,
                StatusStats = statusStats,
                MonthlyRevenue = monthlyRevenue,
                DailyRevenue = dailyRevenue
            };
        }
    }
}
