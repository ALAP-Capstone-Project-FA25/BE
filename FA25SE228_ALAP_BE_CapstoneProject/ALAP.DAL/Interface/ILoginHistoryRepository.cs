using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;

namespace ALAP.DAL.Interface
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

