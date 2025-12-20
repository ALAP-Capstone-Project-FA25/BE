using ALAP.BLL.Interface;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class LessonNoteBizLogic : ILessonNoteBizLogic
    {
        private readonly ILessonNoteRepository _repo;

        public LessonNoteBizLogic(ILessonNoteRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> CreateUpdateLessonNote(CreateUpdateLessonNoteDto dto)
        {
            if (dto.Id > 0)
            {
                var model = new LessonNoteModel
                {
                    Id = dto.Id,
                    LessonId = dto.LessonId,
                    Text = dto.Text,
                    Time = dto.Time,
                    UpdatedAt = Utils.GetCurrentVNTime(),
                };
                return await _repo.Update(model);
            }
            else
            {
                var model = new LessonNoteModel
                {
                    LessonId = dto.LessonId,
                    Text = dto.Text,
                    Time = dto.Time,
                };
                return await _repo.Create(model);
            }
        }

        public async Task<bool> DeleteLessonNote(long id)
        {
            return await _repo.Delete(id);
        }

        public async Task<List<LessonNoteModel>> GetLessonNotesByLessonId(long lessonId)
        {
            return await _repo.GetByLessonId(lessonId);
        }

        public async Task<LessonNoteModel> GetLessonNoteById(long id)
        {
            return await _repo.GetById(id) ?? throw new KeyNotFoundException("Không tìm thấy ghi chú bài học.");
        }

        public async Task<PagedResult<LessonNoteModel>> GetListLessonNotesByPaging(PagingModel pagingModel)
        {
            return await _repo.GetListByPaging(pagingModel);
        }

        public async Task<List<LessonNoteModel>> GetStudentLessonNotes(long studentId, long lessonId)
        {
            // For now, return all notes for the lesson
            // TODO: Add student filtering when user tracking is implemented
            return await _repo.GetByLessonId(lessonId);
        }
    }
}

