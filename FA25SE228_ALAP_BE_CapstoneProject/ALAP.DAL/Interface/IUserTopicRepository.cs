using ALAP.Entity.Models;

namespace ALAP.DAL.Interface
{
    public interface IUserTopicRepository
    {
        Task<UserTopicModel?> Get(long userCourseId, long topicId);
        Task<List<UserTopicModel>> GetByUserCourse(long userCourseId);
        Task<bool> Create(UserTopicModel model);
        Task<bool> Update(UserTopicModel model);
    }
}


