using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;

namespace ALAP.DAL.Interface
{
	public interface ITopicQuestionRepository
	{
		Task<bool> Create(TopicQuestionModel model);
		Task<bool> Update(TopicQuestionModel model);
		Task<TopicQuestionModel?> GetById(long id);
		Task<PagedResult<TopicQuestionModel>> GetListByPaging(PagingModel pagingModel, long topicId);
		Task<bool> Delete(long id);
	}
}


