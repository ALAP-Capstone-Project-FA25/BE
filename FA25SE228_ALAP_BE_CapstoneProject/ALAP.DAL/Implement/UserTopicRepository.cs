using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.DAL.Implement
{
    public class UserTopicRepository : AppBaseRepository, IUserTopicRepository
    {
        private readonly BaseDBContext _dbContext;

        public UserTopicRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserTopicModel?> Get(long userCourseId, long topicId)
        {
            return await _dbContext.UserTopics.FirstOrDefaultAsync(x => x.UserCourseId == userCourseId && x.TopicId == topicId);
        }

        public async Task<List<UserTopicModel>> GetByUserCourse(long userCourseId)
        {
            return await _dbContext.UserTopics
                .Include(x => x.Topic)
                .Where(x => x.UserCourseId == userCourseId)
                .ToListAsync();
        }

        public async Task<bool> Create(UserTopicModel model)
        {
            await _dbContext.UserTopics.AddAsync(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Update(UserTopicModel model)
        {
            _dbContext.UserTopics.Update(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}


