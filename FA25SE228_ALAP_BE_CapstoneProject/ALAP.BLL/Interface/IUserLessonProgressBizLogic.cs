using System.Threading.Tasks;
using System.Collections.Generic;
using ALAP.Entity.Models;
using ALAP.Entity.DTO.Request;

namespace ALAP.BLL.Interface
{
	public interface IUserLessonProgressBizLogic
	{
		Task<UserLessonModel?> Get(long userId, long courseId, long topicId, long lessonId);
		Task<List<UserLessonModel>> GetByTopic(long userId, long courseId, long topicId);
		Task<bool> CreateOrUpdate(long userId, CreateUpdateUserLessonDto dto);
		Task<object> GetUserLessonProgress(long userId, long lessonId);
	}
}
