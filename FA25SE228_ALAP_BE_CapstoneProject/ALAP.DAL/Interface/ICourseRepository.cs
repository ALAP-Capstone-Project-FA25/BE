using App.Entity.Models;
using App.Entity.Models.Wapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DAL.Interface
{
    public interface ICourseRepository
    {
        Task<bool> Create(CourseModel courseModel);
        Task<bool> Update(CourseModel courseModel);
        Task<CourseModel?> GetById(long id);
        Task<PagedResult<CourseModel>> GetListByPaging(PagingModel pagingModel);
        Task<bool> Delete(long id);
    }
}
