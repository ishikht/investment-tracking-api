using InvestmentTracking.Core.Entities;
using InvestmentTracking.Core.Data;

namespace InvestmentTracking.Data.Repositories
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(SqlDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Account>> GetByBrokerIdAsync(Guid brokerId)
        {
            return await FindAsync(a => a.BrokerId == brokerId);
        }

        public async Task<decimal> GetBrokerBalance(Guid brokerId)
        {
            var accounts = await GetByBrokerIdAsync(brokerId);
            return accounts.Sum(a => a.Balance);
        }
    }
}
