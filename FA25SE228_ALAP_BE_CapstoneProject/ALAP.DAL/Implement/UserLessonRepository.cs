using ALAP.DAL;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace ALAP.DAL.Implement
{
    public class UserLessonRepository : AppBaseRepository, IUserLessonRepository
    {
        private readonly BaseDBContext _dbContext;

        public UserLessonRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserLessonModel?> Get(long userTopicId, long lessonId)
        {
            return await _dbContext.UserLessons.FirstOrDefaultAsync(x => x.UserTopicId == userTopicId && x.LessonId == lessonId);
        }

        public async Task<List<UserLessonModel>> GetByUserTopic(long userTopicId)
        {
            return await _dbContext.UserLessons
                .Include(x => x.Lesson)
                .Where(x => x.UserTopicId == userTopicId)
                .ToListAsync();
        }

        public async Task<List<UserLessonModel>> GetByUserCourse(long userCourseId)
        {
            return await _dbContext.UserLessons
                .Where(x => _dbContext.UserTopics.Any(ut => ut.Id == x.UserTopicId && ut.UserCourseId == userCourseId))
                .ToListAsync();
        }

        public async Task<bool> Create(UserLessonModel model)
        {
            await _dbContext.UserLessons.AddAsync(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Update(UserLessonModel model)
        {
            _dbContext.UserLessons.Update(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<LessonModel> GetLessonById(long lessonId)
        {
            return await _dbContext.Lessons.FirstOrDefaultAsync(x => x.Id == lessonId);
        }
    }
}


