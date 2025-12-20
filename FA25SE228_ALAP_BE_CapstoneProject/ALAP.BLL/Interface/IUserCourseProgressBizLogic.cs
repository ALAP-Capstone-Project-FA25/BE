using System.Threading.Tasks;
using System.Collections.Generic;
using ALAP.Entity.Models;
using ALAP.Entity.DTO.Request;

namespace ALAP.BLL.Interface
{
	public interface IUserCourseProgressBizLogic
	{
		Task<bool> Enroll(long userId, long courseId);
		Task<UserCourseModel?> Get(long userId, long courseId);
		Task<List<UserCourseModel>> GetMyCourses(long userId);
		Task<bool> Update(long userId, UpdateUserCourseDto dto);

		Task<bool> UpdateLessonProgress(long topicId, long lessonId, long userId);
		Task<List<UserCourseDto>> GetListUserCourseByCourseId(long courseId);
		Task<object> GetStudentProgress(long userId, long courseId);
		Task<object> GetCurrentLesson(long userId, long courseId);
    }
}
