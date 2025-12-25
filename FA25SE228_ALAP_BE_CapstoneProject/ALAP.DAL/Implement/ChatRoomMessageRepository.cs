using ALAP.DAL;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;

namespace ALAP.DAL.Implement
{
    public class ChatRoomMessageRepository : AppBaseRepository, IChatRoomMessageRepository
    {
        private readonly BaseDBContext _dbContext;

        public ChatRoomMessageRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(ChatRoomMessageModel model)
        {
            await _dbContext.ChatRoomMessages.AddAsync(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Update(ChatRoomMessageModel model)
        {
            _dbContext.ChatRoomMessages.Update(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<ChatRoomMessageModel?> GetById(long id)
        {
            return await _dbContext.ChatRoomMessages
                .Include(m => m.ChatRoom)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedResult<ChatRoomMessageModel>> GetListByPaging(PagingModel pagingModel)
        {
            var query = _dbContext.ChatRoomMessages
                .Include(m => m.ChatRoom)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagingModel.Keyword))
            {
                query = query.Where(m => m.Content.Contains(pagingModel.Keyword));
            }

            query = query.OrderByDescending(m => m.CreatedAt);

            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }

        public async Task<List<ChatRoomMessageModel>> GetByChatRoomId(long chatRoomId)
        {
            //var query = _dbContext.ChatRoomMessages
            //    .Include(m => m.ChatRoom)
            //    .Where(m => m.ChatRoomId == chatRoomId)
            //    .AsQueryable();

            //if (!string.IsNullOrEmpty(pagingModel.Keyword))
            //{
            //    query = query.Where(m => m.Content.Contains(pagingModel.Keyword));
            //}

            //query = query.OrderByDescending(m => m.CreatedAt);

            //return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
            var query = await _dbContext.ChatRoomMessages
                .Where(m => m.ChatRoomId == chatRoomId)
                .ToListAsync();
            return query;

        }

        public async Task<bool> Delete(long id)
        {
            var entity = await _dbContext.ChatRoomMessages.FindAsync(id);
            if (entity == null)
            {
                throw new System.Collections.Generic.KeyNotFoundException("Không tìm thấy tin nhắn.");
            }
            _dbContext.ChatRoomMessages.Remove(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}

