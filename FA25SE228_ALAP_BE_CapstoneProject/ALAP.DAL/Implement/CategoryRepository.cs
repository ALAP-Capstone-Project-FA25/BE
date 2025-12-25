using ALAP.DAL;
using ALAP.DAL.Database;
using ALAP.Entity.Models;
using ALAP.Entity.Models.Wapper;
using ALAP.DAL.Interface;
using Microsoft.EntityFrameworkCore;


namespace ALAP.DAL.Implement
{
    public class CategoryRepository : AppBaseRepository, ICategoryRepository
    {
        private readonly BaseDBContext _dbContext;

        public CategoryRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(CategoryModel categoryModel)
        {
            await _dbContext.Categories.AddAsync(categoryModel);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var existingCategory = await _dbContext.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy danh mục.");
            }
            _dbContext.Categories.Remove(existingCategory);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<CategoryModel?> GetById(long id)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            return category;
        }

        public async Task<PagedResult<CategoryModel>> GetListByPaging(PagingModel pagingModel)
        {
            var query = _dbContext.Categories
                    .Include(x => x.Courses)
                .AsQueryable();

            if(pagingModel.MajorId != 0)
            {
                query = query.Where(c => c.MajorId == pagingModel.MajorId);
            }

            if (!string.IsNullOrEmpty(pagingModel.Keyword))
            {
                query = query.Where(c => c.Name.Contains(pagingModel.Keyword));
            }
           return await QueryableExtensions.ToPagedResultAsync( query,pagingModel);
        }

        public async Task<bool> Update(CategoryModel categoryModel)
        {
            _dbContext.Categories.Update(categoryModel);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
