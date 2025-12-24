using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.Models;
using App.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DAL.Implement
{
	public class TopicQuestionRepository : AppBaseRepository, ITopicQuestionRepository
	{
		private readonly BaseDBContext _dbContext;

		public TopicQuestionRepository(BaseDBContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<bool> Create(TopicQuestionModel model)
		{
			await _dbContext.TopicQuestions.AddAsync(model);
			var result = await _dbContext.SaveChangesAsync();
			return result > 0;
		}

		public async Task<bool> Delete(long id)
		{
			var existing = await _dbContext.TopicQuestions.FindAsync(id);
			if (existing == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy câu hỏi.");
			}
			_dbContext.TopicQuestions.Remove(existing);
			var result = await _dbContext.SaveChangesAsync();
			return result > 0;
		}

		public async Task<TopicQuestionModel?> GetById(long id)
		{
			var item = await _dbContext.TopicQuestions
				.Include(q => q.Topic)
				.Include(q => q.TopicQuestionAnswers)
				.FirstOrDefaultAsync(q => q.Id == id);
			return item;
		}

		public async Task<PagedResult<TopicQuestionModel>> GetListByPaging(PagingModel pagingModel, long topicId)
		{
			var query = _dbContext.TopicQuestions
				.Include(q => q.Topic)
				.Include(q => q.TopicQuestionAnswers)
				.Where(x => x.TopicId == topicId)
				.AsQueryable();

			if (!string.IsNullOrEmpty(pagingModel.Keyword))
			{
				query = query.Where(q => q.Question.Contains(pagingModel.Keyword));
			}

			return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
		}

		public async Task<bool> Update(TopicQuestionModel model)
		{
			_dbContext.TopicQuestions.Update(model);
			var result = await _dbContext.SaveChangesAsync();
			return result > 0;
		}
	}
}


