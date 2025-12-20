using ALAP.BLL.Interface;
using ALAP.DAL.DataBase;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class DashboardBizLogic : AppBaseBizLogic, IDashboardBizLogic
    {
        private readonly BaseDBContext _dbContext;
        private readonly IMapper _mapper;

        public DashboardBizLogic(BaseDBContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<DashboardStatsDto> GetDashboardStats()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            // Total courses
            var totalCourses = await _dbContext.Courses.CountAsync();
            var lastMonthCourses = await _dbContext.Courses
                .Where(c => c.CreatedAt.Month == lastMonth && c.CreatedAt.Year == lastMonthYear)
                .CountAsync();
            var courseGrowth = lastMonthCourses > 0 ? 
                $"+{Math.Round((double)(totalCourses - lastMonthCourses) / lastMonthCourses * 100, 1)}%" : "+0%";

            // Total students (users with role USER)
            var totalStudents = await _dbContext.Users
                .Where(u => u.Role == UserRole.USER)
                .CountAsync();
            var lastMonthStudents = await _dbContext.Users
                .Where(u => u.Role == UserRole.USER && 
                           u.CreatedAt.Month == lastMonth && u.CreatedAt.Year == lastMonthYear)
                .CountAsync();
            var studentGrowth = lastMonthStudents > 0 ? 
                $"+{Math.Round((double)lastMonthStudents / totalStudents * 100, 1)}%" : "+0%";

            // Total revenue from payments
            var totalRevenue = await _dbContext.Payments
                .Where(p => p.PaymentStatus == PaymentStatus.SUCCESS)
                .SumAsync(p => p.Amount);
            var lastMonthRevenue = await _dbContext.Payments
                .Where(p => p.PaymentStatus == PaymentStatus.SUCCESS &&
                           p.CreatedAt.Month == lastMonth && p.CreatedAt.Year == lastMonthYear)
                .SumAsync(p => p.Amount);
            var revenueGrowth = lastMonthRevenue > 0 ? 
                $"+{Math.Round((double)(totalRevenue - lastMonthRevenue) / lastMonthRevenue * 100, 1)}%" : "+0%";

            // Average completion rate
            var userCourses = await _dbContext.UserCourses.ToListAsync();
            var completedCourses = userCourses.Where(uc => uc.IsDone).Count();
            var averageCompletionRate = userCourses.Count > 0 ? 
                (int)Math.Round((double)completedCourses / userCourses.Count * 100) : 0;

            return new DashboardStatsDto
            {
                TotalCourses = totalCourses,
                CourseGrowth = courseGrowth,
                TotalStudents = totalStudents,
                StudentGrowth = studentGrowth,
                TotalRevenue = totalRevenue,
                RevenueGrowth = revenueGrowth,
                AverageCompletionRate = averageCompletionRate,
                CompletionRateGrowth = "+5%" // Mock data for now
            };
        }

        public async Task<RevenueChartDto> GetRevenueChart(string period = "month")
        {
            var result = new RevenueChartDto();
            var currentDate = DateTime.Now;

            if (period == "month")
            {
                // Last 12 months
                for (int i = 11; i >= 0; i--)
                {
                    var date = currentDate.AddMonths(-i);
                    var monthName = $"T{date.Month}";
                    result.Labels.Add(monthName);

                    var revenue = await _dbContext.Payments
                        .Where(p => p.PaymentStatus == PaymentStatus.SUCCESS &&
                                   p.CreatedAt.Month == date.Month && 
                                   p.CreatedAt.Year == date.Year)
                        .SumAsync(p => p.Amount);
                    
                    result.Data.Add(revenue);
                }
            }
            else if (period == "quarter")
            {
                // Last 4 quarters
                for (int i = 3; i >= 0; i--)
                {
                    var quarterStart = currentDate.AddMonths(-i * 3);
                    var quarter = (quarterStart.Month - 1) / 3 + 1;
                    result.Labels.Add($"Q{quarter}");

                    var revenue = await _dbContext.Payments
                        .Where(p => p.PaymentStatus == PaymentStatus.SUCCESS &&
                                   p.CreatedAt.Year == quarterStart.Year &&
                                   ((p.CreatedAt.Month - 1) / 3 + 1) == quarter)
                        .SumAsync(p => p.Amount);
                    
                    result.Data.Add(revenue);
                }
            }
            else // year
            {
                // Last 5 years
                for (int i = 4; i >= 0; i--)
                {
                    var year = currentDate.Year - i;
                    result.Labels.Add(year.ToString());

                    var revenue = await _dbContext.Payments
                        .Where(p => p.PaymentStatus == PaymentStatus.SUCCESS &&
                                   p.CreatedAt.Year == year)
                        .SumAsync(p => p.Amount);
                    
                    result.Data.Add(revenue);
                }
            }

            return result;
        }

        public async Task<StudentGrowthChartDto> GetStudentGrowthChart()
        {
            var result = new StudentGrowthChartDto();
            var currentDate = DateTime.Now;

            // Last 12 months
            for (int i = 11; i >= 0; i--)
            {
                var date = currentDate.AddMonths(-i);
                var monthName = $"T{date.Month}";
                result.Labels.Add(monthName);

                var newStudents = await _dbContext.Users
                    .Where(u => u.Role == UserRole.USER &&
                               u.CreatedAt.Month == date.Month && 
                               u.CreatedAt.Year == date.Year)
                    .CountAsync();
                
                result.Data.Add(newStudents);
            }

            return result;
        }

        public async Task<CourseDistributionDto> GetCourseDistribution()
        {
            var result = new CourseDistributionDto();

            var distribution = await _dbContext.Courses
                .Include(c => c.Category)
                .GroupBy(c => c.Category.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            foreach (var item in distribution)
            {
                result.Labels.Add(item.Category);
                result.Data.Add(item.Count);
            }

            return result;
        }

        public async Task<List<TopCourseDto>> GetTopCourses()
        {
            var topCourses = await _dbContext.Courses
                .Include(c => c.UserCourses)
                .Select(c => new TopCourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    StudentCount = c.UserCourses.Count,
                    CompletionRate = c.UserCourses.Count > 0 ? 
                        (int)Math.Round((double)c.UserCourses.Count(uc => uc.IsDone) / c.UserCourses.Count * 100) : 0
                })
                .OrderByDescending(c => c.CompletionRate)
                .Take(5)
                .ToListAsync();

            return topCourses;
        }

        public async Task<List<RecentCourseDto>> GetRecentCourses(int limit = 10)
        {
            var recentCourses = await _dbContext.Courses
                .Include(c => c.Mentor)
                .Include(c => c.UserCourses)
                .OrderByDescending(c => c.CreatedAt)
                .Take(limit)
                .Select(c => new RecentCourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Instructor = c.Mentor != null ? $"{c.Mentor.FirstName} {c.Mentor.LastName}" : "Chưa có mentor",
                    Students = c.UserCourses.Count,
                    Rating = 4.5m, // Mock rating for now
                    Duration = "8-12 tuần", // Mock duration
                    Status = "active",
                    Thumbnail = "🎓",
                    Progress = c.UserCourses.Count > 0 ? 
                        (int)Math.Round((double)c.UserCourses.Count(uc => uc.IsDone) / c.UserCourses.Count * 100) : 0,
                    Revenue = "₫0" // Mock revenue for now
                })
                .ToListAsync();

            return recentCourses;
        }
    }
}