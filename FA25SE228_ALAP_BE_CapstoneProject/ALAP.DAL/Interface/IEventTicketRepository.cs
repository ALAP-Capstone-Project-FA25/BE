using App.Entity.Models;
using App.Entity.Models.Wapper;
using System.Threading.Tasks;

namespace App.DAL.Interface
{
    public interface IEventTicketRepository
    {
        Task<bool> Create(EventTicketModel model);
        Task<bool> Update(EventTicketModel model);
        Task<bool> Delete(long id);
        Task<EventTicketModel?> GetById(long id);
        Task<PagedResult<EventTicketModel>> GetListByPaging(PagingModel pagingModel, long eventId);
        Task<List<EventTicketModel>> GetByUserId(long userId);
    }
}

