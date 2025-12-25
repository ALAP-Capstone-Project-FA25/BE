using ALAP.BLL.Interface;
using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdaptiveRecommendationController : BaseAPIController
    {
        private readonly IAdaptiveRecommendationBizLogic _recommendationBizLogic;

        public AdaptiveRecommendationController(IAdaptiveRecommendationBizLogic recommendationBizLogic)
        {
            _recommendationBizLogic = recommendationBizLogic;
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetCourseRecommendations()
        {
            try
            {
                var recommendations = await _recommendationBizLogic.GetAdaptiveCourseRecommendations(UserId);
                return GetSuccess(recommendations);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("lessons")]
        public async Task<IActionResult> GetLessonRecommendations()
        {
            try
            {
                var recommendations = await _recommendationBizLogic.GetAdaptiveLessonRecommendations(UserId);
                return GetSuccess(recommendations);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetUserLearningStatistics()
        {
            try
            {
                var statistics = await _recommendationBizLogic.GetUserLearningStatistics(UserId);
                return GetSuccess(statistics);
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }
    }
}
