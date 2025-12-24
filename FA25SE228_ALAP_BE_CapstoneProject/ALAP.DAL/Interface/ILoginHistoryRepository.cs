using App.Entity.Models;
using App.Entity.Models.Wapper;
using System.Threading.Tasks;

namespace App.DAL.Interface
{
    public interface ILoginHistoryRepository
    {
        Task<bool> Create(LoginHistoryModel loginHistoryModel);
        Task<bool> Update(LoginHistoryModel loginHistoryModel);
        Task<LoginHistoryModel?> GetById(long id);
        Task<PagedResult<LoginHistoryModel>> GetListByPaging(PagingModel pagingModel);
        Task<PagedResult<LoginHistoryModel>> GetByUserId(long userId, PagingModel pagingModel);
        Task<bool> Delete(long id);
    }
}

