using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.Models;
using App.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace App.DAL.Implement
{
    public class EventTicketRepository : AppBaseRepository, IEventTicketRepository
    {
        private readonly BaseDBContext _dbContext;

        public EventTicketRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(EventTicketModel model)
        {
            await _dbContext.EventTickets.AddAsync(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Update(EventTicketModel model)
        {
            _dbContext.EventTickets.Update(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var entity = await _dbContext.EventTickets.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Không tìm thấy vé sự kiện.");
            }
            _dbContext.EventTickets.Remove(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<EventTicketModel?> GetById(long id)
        {
            return await _dbContext.EventTickets
                .Include(x => x.Event)
                .Include(x => x.User)
                .Include(x => x.Payment)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedResult<EventTicketModel>> GetListByPaging(PagingModel pagingModel, long eventId)
        {
            var query = _dbContext.EventTickets
                .Include(x => x.Event)
                .Include(x => x.User)
                .Include(x => x.Payment)
                .Where(x => x.EventId == eventId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagingModel.Keyword))
            {
                query = query.Where(x => x.Event.Title.Contains(pagingModel.Keyword)
                    || x.User.Email.Contains(pagingModel.Keyword));
            }

            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }

        public async Task<List<EventTicketModel>> GetByUserId(long userId)
        {
            var data =  await _dbContext.EventTickets
                .Include(x => x.Event)
                .Include(x => x.Payment)
                .Where(x => x.UserId == userId)
                .ToListAsync();
            return data;
        }
    }
}

