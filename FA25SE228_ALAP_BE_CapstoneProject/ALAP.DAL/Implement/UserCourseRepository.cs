using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.DAL.Implement
{
    public class UserCourseRepository : AppBaseRepository, IUserCourseRepository
    {
        private readonly BaseDBContext _dbContext;

        public UserCourseRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserCourseModel?> Get(long userId, long courseId)
        {
            return await _dbContext.UserCourses.FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);
        }

        public async Task<List<UserCourseModel>> GetByUser(long userId)
        {
            return await _dbContext.UserCourses
                .Include(x => x.Course)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> Enroll(UserCourseModel model)
        {
            await _dbContext.UserCourses.AddAsync(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Update(UserCourseModel model)
        {
            _dbContext.UserCourses.Update(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<UserCourseModel>> GetListUserCourseByCourseId(long courseId)
        {
            return await _dbContext.UserCourses
                .Include(x => x.User)
                .Where(x => x.CourseId == courseId)
                .ToListAsync();
        }
    }
}


