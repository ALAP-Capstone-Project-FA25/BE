using ALAP.DAL;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implement
{
	public class TopicQuestionAnswerRepository : AppBaseRepository, ITopicQuestionAnswerRepository
	{
		private readonly BaseDBContext _dbContext;

		public TopicQuestionAnswerRepository(BaseDBContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<bool> Create(TopicQuestionAnswerModel model)
		{
			await _dbContext.TopicQuestionAnswers.AddAsync(model);
			var result = await _dbContext.SaveChangesAsync();
			return result > 0;
		}

		public async Task<bool> Delete(long id)
		{
			var existing = await _dbContext.TopicQuestionAnswers.FindAsync(id);
			if (existing == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy đáp án.");
			}
			_dbContext.TopicQuestionAnswers.Remove(existing);
			var result = await _dbContext.SaveChangesAsync();
			return result > 0;
		}

		public async Task<TopicQuestionAnswerModel?> GetById(long id)
		{
			var item = await _dbContext.TopicQuestionAnswers
				.Include(a => a.TopicQuestion)
				.FirstOrDefaultAsync(a => a.Id == id);
			return item;
		}

		public async Task<PagedResult<TopicQuestionAnswerModel>> GetListByPaging(PagingModel pagingModel, long topicQuestionId)
		{
			var query = _dbContext.TopicQuestionAnswers
				.Include(a => a.TopicQuestion)
				.Where(x => x.TopicQuestionId == topicQuestionId)
				.AsQueryable();

			if (!string.IsNullOrEmpty(pagingModel.Keyword))
			{
				query = query.Where(a => a.Answer.Contains(pagingModel.Keyword));
			}

			return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
		}

		public async Task<bool> Update(TopicQuestionAnswerModel model)
		{
			// Check if entity is already tracked
			var tracked = _dbContext.TopicQuestionAnswers.Local.FirstOrDefault(e => e.Id == model.Id);
			if (tracked != null)
			{
				// Update tracked entity
				tracked.Answer = model.Answer;
				tracked.IsCorrect = model.IsCorrect;
				tracked.UpdatedAt = model.UpdatedAt;
			}
			else
			{
				// Attach and update if not tracked
				_dbContext.TopicQuestionAnswers.Update(model);
			}
			var result = await _dbContext.SaveChangesAsync();
			return result > 0;
		}

		public async Task<List<TopicQuestionAnswerModel>> GetAllByTopicQuestionId(long topicQuestionId)
		{
			return await _dbContext.TopicQuestionAnswers
				.AsNoTracking()
				.Where(x => x.TopicQuestionId == topicQuestionId)
				.ToListAsync();
		}

		public async Task<bool> DeleteRange(IEnumerable<long> ids)
		{
			var entities = await _dbContext.TopicQuestionAnswers
				.Where(x => ids.Contains(x.Id))
				.ToListAsync();
			if (entities.Count == 0)
			{
				return true;
			}
			_dbContext.TopicQuestionAnswers.RemoveRange(entities);
			var result = await _dbContext.SaveChangesAsync();
			return result > 0;
		}
	}
}


