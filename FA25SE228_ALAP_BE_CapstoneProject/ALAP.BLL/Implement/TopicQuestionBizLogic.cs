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
	public class TopicQuestionBizLogic : ITopicQuestionBizLogic
	{
		private readonly ITopicQuestionRepository _repository;
		private readonly ITopicQuestionAnswerRepository _answerRepository;

		public TopicQuestionBizLogic(ITopicQuestionRepository repository, ITopicQuestionAnswerRepository answerRepository)
		{
			_repository = repository;
			_answerRepository = answerRepository;
		}

		public async Task<bool> CreateUpdateTopicQuestion(CreateUpdateTopicQuestionDto dto)
		{
			if (dto.Id > 0)
			{
				var model = new TopicQuestionModel
				{
					Id = dto.Id,
					TopicId = dto.TopicId,
					MaxChoices = dto.MaxChoices,
					Question = dto.Question,
					UpdatedAt = Utils.GetCurrentVNTime(),
				};
				return await _repository.Update(model);
			}
			else
			{
				var model = new TopicQuestionModel
				{
					TopicId = dto.TopicId,
					MaxChoices = dto.MaxChoices,
					Question = dto.Question
				};
				return await _repository.Create(model);
			}
		}

		public async Task<bool> Delete(long id)
		{
			return await _repository.Delete(id);
		}

		public async Task<TopicQuestionModel> GetById(long id)
		{
			return await _repository.GetById(id) ?? throw new KeyNotFoundException($"Không tìm thấy câu hỏi.");
		}

		public async Task<PagedResult<TopicQuestionModel>> GetListByPaging(PagingModel pagingModel, long topicId)
		{
			return await _repository.GetListByPaging(pagingModel, topicId);
		}

		public async Task<bool> CreateUpdateTopicQuestionWithAnswers(CreateUpdateTopicQuestionWithAnswersDto dto)
		{
			// Upsert Question
			if (dto.Id > 0)
			{
				var q = new TopicQuestionModel
				{
					Id = dto.Id,
					TopicId = dto.TopicId,
					MaxChoices = dto.MaxChoices,
					Question = dto.Question,
					ReferrerLessonId = dto.ReferrerLessonId,
					UpdatedAt = Utils.GetCurrentVNTime(),
				};
				await _repository.Update(q);
			}
			else
			{
				var q = new TopicQuestionModel
				{
					TopicId = dto.TopicId,
					MaxChoices = dto.MaxChoices,
					Question = dto.Question,
					ReferrerLessonId = dto.ReferrerLessonId
				};
				await _repository.Create(q);
				var latest = await _repository.GetListByPaging(new PagingModel { PageNumber = 1, PageSize = 1, Keyword = dto.Question }, dto.TopicId);
				var created = latest.ListObjects.FirstOrDefault();
				if (created != null)
				{
					dto.Id = created.Id;
				}
			}

			if (dto.Id <= 0)
			{
				return true;
			}

			var existing = await _answerRepository.GetAllByTopicQuestionId(dto.Id);
			var incomingIds = dto.Answers.Where(a => a.Id > 0).Select(a => a.Id).ToHashSet();
			var toDelete = existing.Where(e => !incomingIds.Contains(e.Id)).Select(e => e.Id).ToList();
			if (toDelete.Count > 0)
			{
				await _answerRepository.DeleteRange(toDelete);
			}

			foreach (var a in dto.Answers)
			{
				if (a.Id > 0)
				{
					// Update existing answer
					await _answerRepository.Update(new TopicQuestionAnswerModel
					{
						Id = a.Id,
						TopicQuestionId = dto.Id,
						Answer = a.Answer,
						IsCorrect = a.IsCorrect,
						UpdatedAt = Utils.GetCurrentVNTime(),
					});
				}
				else
				{
					await _answerRepository.Create(new TopicQuestionAnswerModel
					{
						TopicQuestionId = dto.Id,
						Answer = a.Answer,
						IsCorrect = a.IsCorrect
					});
				}
			}

			return true;
		}
	}
}


