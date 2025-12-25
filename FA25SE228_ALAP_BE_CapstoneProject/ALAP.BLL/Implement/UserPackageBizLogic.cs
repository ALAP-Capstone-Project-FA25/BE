using ALAP.BLL.Interface;
using ALAP.DAL.Database;
using ALAP.Entity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAP.BLL.Implement
{
    public class UserPackageBizLogic : AppBaseBizLogic, IUserPackageBizLogic
    {
        private readonly BaseDBContext _dbContext;

        public UserPackageBizLogic(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserPackageModel> GetUserPackageByUserId(long userId)
        {
            return await _dbContext.UserPackages
                .Where(up => up.UserId == userId && up.IsActive)
                .OrderByDescending(up => up.ExpiredAt)
                .FirstOrDefaultAsync() 
                ?? throw new KeyNotFoundException("Không tìm thấy gói học của người dùng.");
        }
    }
}
