using ALAP.Entity.DTO;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface ICategoryBizLogic
    {
        Task<bool> CreateUpdateCategory(CreateUpdateCategoryDto dto);
        Task<CategoryModel> GetCategoryById(long id);
        Task<PagedResult<CategoryDto>> GetListCategoriesByPaging(PagingModel pagingModel);
        Task<bool> DeleteCategory(long id);
    }
}
