using App.Entity.Models;
using App.Entity.Models.Wapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DAL.Interface
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


