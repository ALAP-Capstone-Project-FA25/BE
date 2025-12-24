using ALAP.Entity.Models;

namespace ALAP.DAL.Interface
{
    public interface IUserLessonRepository
    {
        Task<UserLessonModel?> Get(long userTopicId, long lessonId);
        Task<List<UserLessonModel>> GetByUserTopic(long userTopicId);
        Task<List<UserLessonModel>> GetByUserCourse(long userCourseId);
        Task<LessonModel> GetLessonById(long lessonId);
        Task<bool> Create(UserLessonModel model);
        Task<bool> Update(UserLessonModel model);
    }
}


