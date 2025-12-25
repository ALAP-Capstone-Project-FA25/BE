using ALAP.BLL.Interface;
using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for dashboard
    public class DashboardController : BaseAPIController
    {
        private readonly IDashboardBizLogic _dashboardBizLogic;

        public DashboardController(IDashboardBizLogic dashboardBizLogic)
        {
            _dashboardBizLogic = dashboardBizLogic;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await _dashboardBizLogic.GetDashboardStats();
                return GetSuccess(stats);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("revenue-chart")]
        public async Task<IActionResult> GetRevenueChart([FromQuery] string period = "month")
        {
            try
            {
                var chartData = await _dashboardBizLogic.GetRevenueChart(period);
                return GetSuccess(chartData);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("student-growth")]
        public async Task<IActionResult> GetStudentGrowthChart()
        {
            try
            {
                var chartData = await _dashboardBizLogic.GetStudentGrowthChart();
                return GetSuccess(chartData);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("course-distribution")]
        public async Task<IActionResult> GetCourseDistribution()
        {
            try
            {
                var distribution = await _dashboardBizLogic.GetCourseDistribution();
                return GetSuccess(distribution);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("top-courses")]
        public async Task<IActionResult> GetTopCourses()
        {
            try
            {
                var topCourses = await _dashboardBizLogic.GetTopCourses();
                return GetSuccess(topCourses);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("recent-courses")]
        public async Task<IActionResult> GetRecentCourses([FromQuery] int limit = 10)
        {
            try
            {
                var recentCourses = await _dashboardBizLogic.GetRecentCourses(limit);
                return GetSuccess(recentCourses);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }
    }
}
