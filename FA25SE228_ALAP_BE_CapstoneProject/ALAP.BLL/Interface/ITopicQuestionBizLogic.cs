using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Interface
{
	public interface ITopicQuestionBizLogic
	{
		Task<bool> CreateUpdateTopicQuestion(CreateUpdateTopicQuestionDto dto);
		Task<bool> CreateUpdateTopicQuestionWithAnswers(CreateUpdateTopicQuestionWithAnswersDto dto);
		Task<TopicQuestionModel> GetById(long id);
		Task<PagedResult<TopicQuestionModel>> GetListByPaging(PagingModel pagingModel, long topicId);
		Task<bool> Delete(long id);
	}
}


