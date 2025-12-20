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
    public interface ILessonBizLogic
    {
        Task<bool> CreateUpdateLesson(CreateUpdateLessonDto dto);
        Task<LessonModel> GetLessonById(long id);
        Task<PagedResult<LessonModel>> GetListLessonsByPaging(PagingModel pagingModel, long topicId);
        Task<bool> DeleteLesson(long id);
    }
}
