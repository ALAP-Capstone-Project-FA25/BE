using Microsoft.EntityFrameworkCore;
using ALAP.DAL;
using ALAP.DAL.Database;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using ALAP.DAL.Interface;

namespace ALAP.DAL.Implement
{
    public class MajorRepository : AppBaseRepository, IMajorRepository
    {
        private readonly BaseDBContext _dbContext;

        public MajorRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(MajorModel model)
        {
            await _dbContext.Majors.AddAsync(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var existing = await _dbContext.Majors.FindAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException("Không tìm thấy ngành học.");
            }
            _dbContext.Majors.Remove(existing);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<MajorModel?> GetById(long id)
        {
            return await _dbContext.Majors
                .Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedResult<MajorModel>> GetListByPaging(PagingModel pagingModel)
        {
            var query = _dbContext.Majors
                .Include(x => x.Categories)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagingModel.Keyword))
            {
                query = query.Where(x => x.Name.Contains(pagingModel.Keyword));
            }

            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }

        public async Task<bool> Update(MajorModel model)
        {
            _dbContext.Majors.Update(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}

