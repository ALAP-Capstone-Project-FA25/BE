using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;

namespace App.DAL.Interface
{
    public interface ICategoryRepository
    {
        Task<bool> Create(CategoryModel categoryModel);
        Task<bool> Update(CategoryModel categoryModel);
        Task<CategoryModel?> GetById(long id);
        Task<PagedResult<CategoryModel>> GetListByPaging(PagingModel pagingModel);
        Task<bool> Delete(long id);
    }
}
