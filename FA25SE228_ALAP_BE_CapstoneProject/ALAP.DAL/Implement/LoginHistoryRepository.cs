using ALAP.DAL;
using ALAP.DAL.Database;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implement
{
    public class LoginHistoryRepository : AppBaseRepository, ILoginHistoryRepository
    {
        private readonly BaseDBContext _dbContext;

        public LoginHistoryRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(LoginHistoryModel loginHistoryModel)
        {
            await _dbContext.LoginHistories.AddAsync(loginHistoryModel);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var existingLoginHistory = await _dbContext.LoginHistories.FindAsync(id);
            if (existingLoginHistory == null)
            {
                throw new System.Collections.Generic.KeyNotFoundException($"Không tìm thấy lịch sử đăng nhập.");
            }
            _dbContext.LoginHistories.Remove(existingLoginHistory);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<LoginHistoryModel?> GetById(long id)
        {
            var loginHistory = await _dbContext.LoginHistories
                .Include(lh => lh.User)
                .FirstOrDefaultAsync(lh => lh.Id == id);
            return loginHistory;
        }

        public async Task<PagedResult<LoginHistoryModel>> GetListByPaging(PagingModel pagingModel)
        {
            var query = _dbContext.LoginHistories
                .Include(lh => lh.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagingModel.Keyword))
            {
                query = query.Where(lh => 
                    lh.IpAddress.Contains(pagingModel.Keyword) || 
                    lh.User.Username.Contains(pagingModel.Keyword) ||
                    lh.User.Email.Contains(pagingModel.Keyword));
            }

            if (pagingModel.CourseId > 0)
            {
                query = query.Where(lh => lh.UserId == pagingModel.CourseId);
            }

            query = query.OrderByDescending(lh => lh.LoginDate);

            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }

        public async Task<PagedResult<LoginHistoryModel>> GetByUserId(long userId, PagingModel pagingModel)
        {
            var query = _dbContext.LoginHistories
                .Include(lh => lh.User)
                .Where(lh => lh.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagingModel.Keyword))
            {
                query = query.Where(lh =>
                    lh.IpAddress.Contains(pagingModel.Keyword));
            }

            query = query.OrderByDescending(lh => lh.LoginDate);

            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }

        public async Task<bool> Update(LoginHistoryModel loginHistoryModel)
        {
            _dbContext.LoginHistories.Update(loginHistoryModel);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}

