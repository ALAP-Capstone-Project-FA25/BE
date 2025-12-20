using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;

namespace ALAP.BLL.Interface
{
    public interface IPackageBizLogic
    {
        Task<bool> CreateUpdatePackage(CreateUpdatePackageDto dto);
        Task<PackageModel> GetPackageById(long id);
        Task<PagedResult<PackageModel>> GetListPackagesByPaging(PagingModel pagingModel);
        Task<bool> DeletePackage(long id);

        Task<string> BuyPackage(long packageId, long userId);
    }
}


