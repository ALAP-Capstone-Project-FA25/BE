using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;

namespace ALAP.DAL.Interface
{
    public interface IMajorRepository
    {
        Task<bool> Create(MajorModel model);
        Task<bool> Update(MajorModel model);
        Task<MajorModel?> GetById(long id);
        Task<PagedResult<MajorModel>> GetListByPaging(PagingModel pagingModel);
        Task<bool> Delete(long id);
    }
}

