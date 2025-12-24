using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;

namespace ALAP.DAL.Interface
{
    public interface IEventRepository
    {
        Task<bool> Create(EventModel model);
        Task<bool> Update(EventModel model);
        Task<bool> Delete(long id);
        Task<EventModel?> GetById(long id);
        Task<PagedResult<EventModel>> GetListByPaging(PagingModel pagingModel);
    }
}

