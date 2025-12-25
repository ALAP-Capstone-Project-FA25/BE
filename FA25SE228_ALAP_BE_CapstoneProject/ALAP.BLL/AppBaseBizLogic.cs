using ALAP.DAL.Database;

namespace ALAP.BLL
{
    public class AppBaseBizLogic
    {
        protected readonly BaseDBContext _dbContext;

        protected AppBaseBizLogic(BaseDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected async Task<T> TransactionAsync<T>(Func<Task<T>> work)
        {
            await using var tx = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var result = await work();

                await _dbContext.SaveChangesAsync();
                await tx.CommitAsync();
                return result;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        protected async Task TransactionAsync(Func<Task> work)
        {
            await using var tx = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await work();

                await _dbContext.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
