using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface ITopicQuizBizLogic
    {
        Task<QuizResultDto> SubmitTopicQuiz(long userId, SubmitTopicQuizDto dto);
        Task<List<SuggestedLessonDto>> GetSuggestedLessons(long userId, long topicId);
    }
}

