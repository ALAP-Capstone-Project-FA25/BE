using ALAP.BLL.Interface;
using ALAP.DAL.Interface;
using ALAP.Entity.DTO.Request;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Base.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
	public class TopicQuestionAnswerBizLogic : ITopicQuestionAnswerBizLogic
	{
		private readonly ITopicQuestionAnswerRepository _repository;

		public TopicQuestionAnswerBizLogic(ITopicQuestionAnswerRepository repository)
		{
			_repository = repository;
		}

		public async Task<bool> CreateUpdateTopicQuestionAnswer(CreateUpdateTopicQuestionAnswerDto dto)
		{
			if (dto.Id > 0)
			{
				var model = new TopicQuestionAnswerModel
				{
					Id = dto.Id,
					TopicQuestionId = dto.TopicQuestionId,
					Answer = dto.Answer,
					IsCorrect = dto.IsCorrect,
					UpdatedAt = Utils.GetCurrentVNTime(),
				};
				return await _repository.Update(model);
			}
			else
			{
				var model = new TopicQuestionAnswerModel
				{
					TopicQuestionId = dto.TopicQuestionId,
					Answer = dto.Answer,
					IsCorrect = dto.IsCorrect
				};
				return await _repository.Create(model);
			}
		}

		public async Task<bool> Delete(long id)
		{
			return await _repository.Delete(id);
		}

		public async Task<TopicQuestionAnswerModel> GetById(long id)
		{
			return await _repository.GetById(id) ?? throw new KeyNotFoundException($"Không tìm thấy đáp án.");
		}

		public async Task<PagedResult<TopicQuestionAnswerModel>> GetListByPaging(PagingModel pagingModel, long topicQuestionId)
		{
			return await _repository.GetListByPaging(pagingModel, topicQuestionId);
		}
	}
}


