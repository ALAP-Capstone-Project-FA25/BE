using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;

namespace ALAP.DAL.Interface
{
	public interface ITopicQuestionAnswerRepository
	{
		Task<bool> Create(TopicQuestionAnswerModel model);
		Task<bool> Update(TopicQuestionAnswerModel model);
		Task<TopicQuestionAnswerModel?> GetById(long id);
		Task<PagedResult<TopicQuestionAnswerModel>> GetListByPaging(PagingModel pagingModel, long topicQuestionId);
		Task<bool> Delete(long id);
		Task<List<TopicQuestionAnswerModel>> GetAllByTopicQuestionId(long topicQuestionId);
		Task<bool> DeleteRange(IEnumerable<long> ids);
	}
}


