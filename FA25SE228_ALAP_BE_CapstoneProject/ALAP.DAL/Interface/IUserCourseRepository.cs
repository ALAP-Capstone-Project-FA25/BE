using ALAP.Entity.Models;

namespace ALAP.DAL.Interface
{
    public interface IUserCourseRepository
    {
        Task<UserCourseModel?> Get(long userId, long courseId);
        Task<List<UserCourseModel>> GetByUser(long userId);
        Task<bool> Enroll(UserCourseModel model);
        Task<bool> Update(UserCourseModel model);
        Task<List<UserCourseModel>> GetListUserCourseByCourseId(long courseId);

    }
}


