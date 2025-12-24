using ALAP.DAL;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implement
{
    public class UserWeakAreaRepository : AppBaseRepository, IUserWeakAreaRepository
    {
        private readonly BaseDBContext _dbContext;

        public UserWeakAreaRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserWeakArea?> GetByUserAndLesson(long userId, long lessonId)
        {
            return await _dbContext.UserWeakAreas
                .FirstOrDefaultAsync(w => w.UserId == userId && w.LessonId == lessonId);
        }

        public async Task<UserWeakArea> CreateOrUpdate(UserWeakArea weakArea)
        {
            var existing = await GetByUserAndLesson(weakArea.UserId, weakArea.LessonId);
            
            if (existing != null)
            {
                existing.ReferralCount += 1;
                existing.LastReferralAt = weakArea.LastReferralAt;
                existing.MasteryLevel = weakArea.MasteryLevel;
                existing.UpdatedAt = Base.Common.Utils.GetCurrentVNTime();
                _dbContext.UserWeakAreas.Update(existing);
            }
            else
            {
                await _dbContext.UserWeakAreas.AddAsync(weakArea);
            }
            
            await _dbContext.SaveChangesAsync();
            return existing ?? weakArea;
        }

        public async Task<List<UserWeakArea>> GetByUserId(long userId)
        {
            return await _dbContext.UserWeakAreas
                .Include(w => w.Lesson)
                .Include(w => w.Course)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.ReferralCount)
                .ToListAsync();
        }

        public async Task<List<UserWeakArea>> GetByCourseId(long userId, long courseId)
        {
            return await _dbContext.UserWeakAreas
                .Include(w => w.Lesson)
                .Where(w => w.UserId == userId && w.CourseId == courseId)
                .OrderByDescending(w => w.ReferralCount)
                .ToListAsync();
        }

        public async Task<List<UserWeakArea>> GetTopWeakAreas(long userId, int topCount)
        {
            return await _dbContext.UserWeakAreas
                .Include(w => w.Lesson)
                .Include(w => w.Course)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.ReferralCount)
                .ThenBy(w => w.MasteryLevel)
                .Take(topCount)
                .ToListAsync();
        }
    }
}

