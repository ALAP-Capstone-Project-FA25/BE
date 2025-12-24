using App.Entity.Models;
using App.Entity.Models.Wapper;
using System.Threading.Tasks;

namespace App.DAL.Interface
{
    public interface ILessonNoteRepository
    {
        Task<bool> Create(LessonNoteModel model);
        Task<bool> Update(LessonNoteModel model);
        Task<LessonNoteModel?> GetById(long id);
        Task<PagedResult<LessonNoteModel>> GetListByPaging(PagingModel pagingModel);
        Task<List<LessonNoteModel>> GetByLessonId(long lessonId);
        Task<bool> Delete(long id);
    }
}

