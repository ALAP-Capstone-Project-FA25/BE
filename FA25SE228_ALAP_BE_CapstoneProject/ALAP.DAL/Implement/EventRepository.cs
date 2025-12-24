using ALAP.DAL;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implement
{
    public class EventRepository : AppBaseRepository, IEventRepository
    {
        private readonly BaseDBContext _dbContext;

        public EventRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(EventModel model)
        {
            await _dbContext.Events.AddAsync(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Update(EventModel model)
        {
            _dbContext.Events.Update(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var entity = await _dbContext.Events.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Không tìm thấy sự kiện.");
            }
            _dbContext.Events.Remove(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<EventModel?> GetById(long id)
        {
            return await _dbContext.Events
                .Include(x => x.Speaker)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedResult<EventModel>> GetListByPaging(PagingModel pagingModel)
        {
            var query = _dbContext.Events
                .Include(x => x.Speaker)
                .AsQueryable();
            if (!string.IsNullOrEmpty(pagingModel.Keyword))
            {
                query = query.Where(x => x.Title.Contains(pagingModel.Keyword));
            }
            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }
    }
}

