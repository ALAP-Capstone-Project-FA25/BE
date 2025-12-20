using ALAP.Entity.DTO.Request;
using ALAP.Entity.DTO.Response;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface ICourseBizLogic
    {
        Task<bool> CreateUpdateCourse(CreateUpdateCourseDto dto);
        Task<CourseModel> GetCourseById(long id);
        Task<PagedResult<CourseDto>> GetListCoursesByPaging(PagingModel pagingModel);
        Task<List<CourseModel>> GetListCourseByCategoryId(long categoryId);
        Task<bool> DeleteCourse(long id);
        Task<bool> AssignMentorToCourse(long courseId, long mentorId);
    }
}
