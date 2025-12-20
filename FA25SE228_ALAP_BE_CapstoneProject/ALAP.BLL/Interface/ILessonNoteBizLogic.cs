using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
    public interface ILessonNoteBizLogic
    {
        Task<bool> CreateUpdateLessonNote(CreateUpdateLessonNoteDto dto);
        Task<LessonNoteModel> GetLessonNoteById(long id);
        Task<PagedResult<LessonNoteModel>> GetListLessonNotesByPaging(PagingModel pagingModel);
        Task<List<LessonNoteModel>> GetLessonNotesByLessonId(long lessonId);
        Task<List<LessonNoteModel>> GetStudentLessonNotes(long studentId, long lessonId);
        Task<bool> DeleteLessonNote(long id);
    }
}

