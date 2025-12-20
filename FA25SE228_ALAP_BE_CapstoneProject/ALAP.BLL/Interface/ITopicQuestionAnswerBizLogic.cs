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
	public interface ITopicQuestionAnswerBizLogic
	{
		Task<bool> CreateUpdateTopicQuestionAnswer(CreateUpdateTopicQuestionAnswerDto dto);
		Task<TopicQuestionAnswerModel> GetById(long id);
		Task<PagedResult<TopicQuestionAnswerModel>> GetListByPaging(PagingModel pagingModel, long topicQuestionId);
		Task<bool> Delete(long id);
	}
}


