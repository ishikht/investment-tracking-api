using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;

namespace InvestmentTracking.Data.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(SqlDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId)
        {
            return await FindAsync(t => t.AccountId == accountId);
        }
    }
}
