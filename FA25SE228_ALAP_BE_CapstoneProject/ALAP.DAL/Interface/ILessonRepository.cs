using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;

namespace ALAP.DAL.Interface
{
    public interface ILessonRepository
    {
        Task<bool> Create(LessonModel lessonModel);
        Task<bool> Update(LessonModel lessonModel);
        Task<LessonModel?> GetById(long id);
        Task<PagedResult<LessonModel>> GetListByPaging(PagingModel pagingModel, long topicId);
        Task<bool> Delete(long id);
        Task<int> GetLastOrderIndexByTopic(long topicId);
    }
}
