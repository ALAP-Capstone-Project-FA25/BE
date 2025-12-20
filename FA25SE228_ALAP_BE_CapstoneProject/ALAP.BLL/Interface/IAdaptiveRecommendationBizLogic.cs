using ALAP.Entity.DTO.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface IAdaptiveRecommendationBizLogic
    {
        Task<List<AdaptiveCourseRecommendationDto>> GetAdaptiveCourseRecommendations(long userId);
        Task<List<AdaptiveLessonRecommendationDto>> GetAdaptiveLessonRecommendations(long userId);
        Task<UserLearningStatisticsDto> GetUserLearningStatistics(long userId);
    }
}

