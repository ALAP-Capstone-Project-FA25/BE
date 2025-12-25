using ALAP.DAL;
using ALAP.DAL.Database;
using ALAP.DAL.Interface;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using Microsoft.EntityFrameworkCore;

namespace ALAP.DAL.Implement
{
    public class EntryTestRepository : AppBaseRepository, IEntryTestRepository
    {
        private readonly BaseDBContext _dbContext;

        public EntryTestRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(EntryTestModel model)
        {
            // Auto set display order if not provided
            if (model.DisplayOrder == 0)
            {
                var maxOrder = await _dbContext.EntryTests
                    .OrderByDescending(x => x.DisplayOrder)
                    .Select(x => x.DisplayOrder)
                    .FirstOrDefaultAsync();
                model.DisplayOrder = maxOrder + 1;
            }
            
            await _dbContext.EntryTests.AddAsync(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var existing = await _dbContext.EntryTests.FindAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException("Không tìm thấy bài test.");
            }
            _dbContext.EntryTests.Remove(existing);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<EntryTestModel?> GetById(long id)
        {
            return await _dbContext.EntryTests.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<EntryTestModel?> GetByIdWithDetails(long id)
        {
            return await _dbContext.EntryTests
                .Include(x => x.Questions.OrderBy(q => q.DisplayOrder))
                    .ThenInclude(q => q.Options.OrderBy(o => o.DisplayOrder))
                        .ThenInclude(o => o.SubjectMappings)
                            .ThenInclude(sm => sm.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<EntryTestModel?> GetActiveTest()
        {
            return await _dbContext.EntryTests
                .Include(x => x.Questions.OrderBy(q => q.DisplayOrder))
                    .ThenInclude(q => q.Options.OrderBy(o => o.DisplayOrder))
                        .ThenInclude(o => o.SubjectMappings)
                            .ThenInclude(sm => sm.Category)
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<EntryTestModel>> GetListByPaging(PagingModel pagingModel)
        {
            var query = _dbContext.EntryTests.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagingModel.Keyword))
            {
                query = query.Where(x => x.Title.Contains(pagingModel.Keyword));
            }

            return await QueryableExtensions.ToPagedResultAsync(query, pagingModel);
        }

        public async Task<bool> Update(EntryTestModel model)
        {
            _dbContext.EntryTests.Update(model);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
