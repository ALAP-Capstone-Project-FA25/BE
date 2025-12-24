using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.Models;
using App.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implement
{
    public class PackageRepository : AppBaseRepository, IPackageRepository
    {
        private readonly BaseDBContext _dbContext;

        public PackageRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(PackageModel model)
        {
            await _dbContext.Packages.AddAsync(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Update(PackageModel model)
        {
            _dbContext.Packages.Update(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var entity = await _dbContext.Packages.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói học.");
            }
            _dbContext.Packages.Remove(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<PackageModel?> GetById(long id)
        {
            return await _dbContext.Packages.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedResult<PackageModel>> GetListByPaging(PagingModel pagingModel)
        {
            var query = _dbContext.Packages.AsQueryable();
            if (!string.IsNullOrEmpty(pagingModel.Keyword))
            {
                query = query.Where(x => x.Title.Contains(pagingModel.Keyword));
            }
            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }
    }
}


