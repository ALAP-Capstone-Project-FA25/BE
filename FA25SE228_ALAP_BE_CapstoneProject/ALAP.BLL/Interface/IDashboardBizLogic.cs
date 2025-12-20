using ALAP.Entity.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IDashboardBizLogic
    {
        Task<DashboardStatsDto> GetDashboardStats();
        Task<RevenueChartDto> GetRevenueChart(string period = "month");
        Task<StudentGrowthChartDto> GetStudentGrowthChart();
        Task<CourseDistributionDto> GetCourseDistribution();
        Task<List<TopCourseDto>> GetTopCourses();
        Task<List<RecentCourseDto>> GetRecentCourses(int limit = 10);
    }
}