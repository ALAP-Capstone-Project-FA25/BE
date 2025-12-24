using App.Entity.Models;
using App.Entity.Models.Wapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
