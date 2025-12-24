using ALAP.DAL;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implement
{
    public class LessonRepository : AppBaseRepository, ILessonRepository
    {
        private readonly BaseDBContext _dbContext;

        public LessonRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(LessonModel lessonModel)
        {
            await _dbContext.Lessons.AddAsync(lessonModel);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var existingLesson = await _dbContext.Lessons.FindAsync(id);
            if (existingLesson == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy bài học.");
            }
            _dbContext.Lessons.Remove(existingLesson);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<LessonModel?> GetById(long id)
        {
            var lesson = await _dbContext.Lessons
                .Include(l => l.Topic)
                .FirstOrDefaultAsync(l => l.Id == id);
            return lesson;
        }

        public async Task<int> GetLastOrderIndexByTopic(long topicId)
        {
            var lastOrderIndex = await _dbContext.Lessons
                .Where(l => l.TopicId == topicId)
                .MaxAsync(l => (int?)l.OrderIndex) ?? 0;
            return lastOrderIndex;

        }

        public async Task<PagedResult<LessonModel>> GetListByPaging(PagingModel pagingModel, long topicId)
        {
            var query = _dbContext.Lessons
                .Include(l => l.Topic)
                .Where(x => x.TopicId == topicId)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(pagingModel.Keyword))
            {
                query = query.Where(l => l.Title.Contains(pagingModel.Keyword) || l.Description.Contains(pagingModel.Keyword));
            }
            
            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }

        public async Task<bool> Update(LessonModel lessonModel)
        {
            _dbContext.Lessons.Update(lessonModel);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
