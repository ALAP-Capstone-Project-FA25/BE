using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface ILoginHistoryBizLogic
    {
        Task<bool> CreateUpdateLoginHistory(CreateUpdateLoginHistoryDto dto);
        Task<LoginHistoryModel> GetLoginHistoryById(long id);
        Task<PagedResult<LoginHistoryModel>> GetListLoginHistoryByPaging(PagingModel pagingModel);
        Task<PagedResult<LoginHistoryModel>> GetLoginHistoryByUserId(long userId, PagingModel pagingModel);
        Task<bool> DeleteLoginHistory(long id);
    }
}

