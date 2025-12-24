using App.Entity.Models;
using App.Entity.Models.Wapper;

namespace App.DAL.Interface
{
    public interface IPackageRepository
    {
        Task<bool> Create(PackageModel model);
        Task<bool> Update(PackageModel model);
        Task<bool> Delete(long id);
        Task<PackageModel?> GetById(long id);
        Task<PagedResult<PackageModel>> GetListByPaging(PagingModel pagingModel);
    }
}


