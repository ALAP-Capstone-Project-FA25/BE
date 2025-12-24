using App.Entity.Models;
using App.Entity.Models.Wapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DAL.Interface
{
    public interface ILessonRepository
    {
        Task<bool> Create(LessonModel lessonModel);
        Task<bool> Update(LessonModel lessonModel);
        Task<LessonModel?> GetById(long id);
        Task<PagedResult<LessonModel>> GetListByPaging(PagingModel pagingModel, long topicId);
        Task<bool> Delete(long id);
        Task<int> GetLastOrderIndexByTopic(long topicId);
    }
}
