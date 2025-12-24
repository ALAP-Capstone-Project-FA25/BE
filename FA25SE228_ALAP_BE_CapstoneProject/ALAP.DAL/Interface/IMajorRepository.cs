using App.Entity.Models;
using App.Entity.Models.Wapper;
using System.Threading.Tasks;

namespace App.DAL.Interface
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

