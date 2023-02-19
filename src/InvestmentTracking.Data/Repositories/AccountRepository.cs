using InvestmentTracking.Core.Entities;
using InvestmentTracking.Core.Data;
using Microsoft.Extensions.Logging;

namespace InvestmentTracking.Data.Repositories;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    private readonly ILogger<AccountRepository> _logger;

    public AccountRepository(SqlDbContext dbContext, ILogger<AccountRepository> logger) : base(dbContext, logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<Account>> GetByBrokerIdAsync(Guid brokerId)
    {
        var accounts = await FindAsync(a => a.BrokerId == brokerId);
        _logger.LogDebug("Retrieved accounts {@Accounts} from DbSet for broker Id {BrokerId}", accounts, brokerId);
        return accounts;
    }

    public async Task<decimal> GetBrokerBalance(Guid brokerId)
    {
        var accounts = await GetByBrokerIdAsync(brokerId);
        var balance = accounts.Sum(a => a.Balance);
        _logger.LogDebug("Retrieved broker balance {Balance:C} from accounts {@Accounts} for broker Id {BrokerId}", balance, accounts, brokerId);
        return balance;
    }
}