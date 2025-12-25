using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;

namespace ALAP.DAL.Interface
{
    public interface ITopicRepository
    {
        Task<bool> Create(TopicModel topicModel);
        Task<bool> Update(TopicModel topicModel);
        Task<TopicModel?> GetById(long id);
        Task<PagedResult<TopicModel>> GetListByPaging(PagingModel pagingModel);
        Task<PagedResult<UserTopicDto>> GetListByPagingByUser(PagingModel pagingModel);

        Task<bool> Delete(long id);
    }
}
