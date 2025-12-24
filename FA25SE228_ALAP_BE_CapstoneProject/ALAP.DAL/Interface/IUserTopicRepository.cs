using App.Entity.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace App.DAL.Interface
{
    public interface IUserTopicRepository
    {
        Task<UserTopicModel?> Get(long userCourseId, long topicId);
        Task<List<UserTopicModel>> GetByUserCourse(long userCourseId);
        Task<bool> Create(UserTopicModel model);
        Task<bool> Update(UserTopicModel model);
    }
}


