using App.Entity.Models;
using App.Entity.Models.Wapper;
using System.Threading.Tasks;

namespace App.DAL.Interface
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

