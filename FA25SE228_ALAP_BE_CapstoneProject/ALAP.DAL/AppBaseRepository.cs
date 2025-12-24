using ALAP.DAL.Database;
using Microsoft.EntityFrameworkCore.Storage;


namespace ALAP.DAL
{
    public class AppBaseRepository : IDisposable
    {
        private readonly BaseDBContext _dbContext;
        private IDbContextTransaction _currentTransaction;

        public AppBaseRepository(BaseDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task BeginTransaction()
        {
            if (_currentTransaction == null)
            {
                _currentTransaction = await _dbContext.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitTransaction()
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync();
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
            catch (Exception ex)
            {
                await RollbackTransaction();
                throw new Exception($"Error committing transaction: {ex.Message}", ex);
            }
        }

        public async Task RollbackTransaction()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _dbContext?.Dispose();
        }

        public void IsNullOrEmpty(string value, string message)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(message);
            }
        }
    }
}
