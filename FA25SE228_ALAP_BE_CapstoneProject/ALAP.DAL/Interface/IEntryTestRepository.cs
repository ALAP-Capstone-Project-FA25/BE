using App.Entity.Models;
using App.Entity.Models.Wapper;
using System.Threading.Tasks;

namespace App.DAL.Interface
{
    public interface IEntryTestRepository
    {
        Task<bool> Create(EntryTestModel model);
        Task<bool> Update(EntryTestModel model);
        Task<EntryTestModel?> GetById(long id);
        Task<EntryTestModel?> GetByIdWithDetails(long id);
        Task<PagedResult<EntryTestModel>> GetListByPaging(PagingModel pagingModel);
        Task<bool> Delete(long id);
        Task<EntryTestModel?> GetActiveTest();
    }
}
