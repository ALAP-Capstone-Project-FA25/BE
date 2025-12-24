using App.DAL;
using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.Models;
using App.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace App.DAL.Implement
{
    public class LessonNoteRepository : AppBaseRepository, ILessonNoteRepository
    {
        private readonly BaseDBContext _dbContext;

        public LessonNoteRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(LessonNoteModel model)
        {
            await _dbContext.LessonNotes.AddAsync(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Update(LessonNoteModel model)
        {
            _dbContext.LessonNotes.Update(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<LessonNoteModel?> GetById(long id)
        {
            return await _dbContext.LessonNotes
                .Include(n => n.Lesson)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<PagedResult<LessonNoteModel>> GetListByPaging(PagingModel pagingModel)
        {
            var query = _dbContext.LessonNotes
                .Include(n => n.Lesson)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagingModel.Keyword))
            {
                query = query.Where(n => n.Text.Contains(pagingModel.Keyword));
            }

            query = query.OrderByDescending(n => n.CreatedAt);

            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }

        public async Task<List<LessonNoteModel>> GetByLessonId(long lessonId)
        {
            var query = _dbContext.LessonNotes
                .Include(n => n.Lesson)
                .Where(n => n.LessonId == lessonId)
                .AsQueryable();

            query = query.OrderBy(n => n.Time);

            return await query.ToListAsync();

        }

        public async Task<bool> Delete(long id)
        {
            var entity = await _dbContext.LessonNotes.FindAsync(id);
            if (entity == null)
            {
                throw new System.Collections.Generic.KeyNotFoundException("Không tìm thấy ghi chú bài học.");
            }
            _dbContext.LessonNotes.Remove(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}

