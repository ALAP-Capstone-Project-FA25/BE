using App.DAL;
using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.Models;
using App.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.DAL.Implement
{
    public class ChatRoomRepository : AppBaseRepository, IChatRoomRepository
    {
        private readonly BaseDBContext _dbContext;

        public ChatRoomRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(ChatRoomModel model)
        {
            await _dbContext.ChatRooms.AddAsync(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Update(ChatRoomModel model)
        {
            _dbContext.ChatRooms.Update(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<ChatRoomModel?> GetById(long id)
        {
            return await _dbContext.ChatRooms
                .Include(cr => cr.CreatedBy)
                .Include(cr => cr.Participant)
                .Include(cr => cr.Messages)
                .FirstOrDefaultAsync(cr => cr.Id == id);
        }

        public async Task<PagedResult<ChatRoomModel>> GetListByPaging(PagingModel pagingModel)
        {
            var query = _dbContext.ChatRooms
                .Include(cr => cr.CreatedBy)
                .Include(cr => cr.Participant)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagingModel.Keyword))
            {
                query = query.Where(cr => cr.Name.Contains(pagingModel.Keyword) ||
                                           cr.CreatedBy.Username.Contains(pagingModel.Keyword) ||
                                           cr.Participant.Username.Contains(pagingModel.Keyword));
            }

            if (pagingModel.UserId > 0)
            {
                var uid = pagingModel.UserId;
                query = query.Where(cr => cr.CreatedById == uid || cr.ParticipantId == uid);
            }

            query = query.OrderByDescending(cr => cr.UpdatedAt).ThenByDescending(cr => cr.CreatedAt);

            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }

        public async Task<bool> Delete(long id)
        {
            var entity = await _dbContext.ChatRooms.FindAsync(id);
            if (entity == null)
            {
                throw new System.Collections.Generic.KeyNotFoundException("Không tìm thấy phòng chat.");
            }
            _dbContext.ChatRooms.Remove(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<ChatRoomModel>> GetByMentorId(long mentorId)
        {
            return await _dbContext.ChatRooms
                .Include(cr => cr.CreatedBy)
                .Include(cr => cr.Participant)
                .Include(cr => cr.Course)
                .Where(cr => cr.ParticipantId == mentorId)
                .OrderByDescending(cr => cr.UpdatedAt)
                .ToListAsync();
        }
    }
}

