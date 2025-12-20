using System.Threading.Tasks;
using System.Collections.Generic;
using ALAP.Entity.Models;
using ALAP.Entity.DTO.Request;

namespace App.BLL.Interface
{
	public interface IUserTopicProgressBizLogic
	{
		Task<UserTopicModel?> Get(long userId, long courseId, long topicId);
		Task<List<UserTopicModel>> GetByCourse(long userId, long courseId);
		Task<bool> CreateOrUpdate(long userId, CreateUpdateUserTopicDto dto);
	}
}
