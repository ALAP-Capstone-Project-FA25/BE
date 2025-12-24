using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;

namespace ALAP.DAL.Interface
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
