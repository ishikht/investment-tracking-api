using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;
using Microsoft.Extensions.Logging;

namespace InvestmentTracking.Data.Repositories;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    private readonly ILogger<AccountRepository> _logger;

    public AccountRepository(SqlDbContext dbContext, ILogger<AccountRepository> logger) : base(dbContext, logger)
    {
        _logger = logger;
    }

    public override async Task AddAsync(Account account)
    {
        if (account.Name == null) throw new ArgumentNullException(nameof(account), "Account name cannot be null");

        if (string.IsNullOrEmpty(account.Name)) throw new ArgumentException("Account name cannot be null or empty");

        await base.AddAsync(account);
    }

    public override Task UpdateAsync(Account account)
    {
        if (account.Name == null) throw new ArgumentNullException(nameof(account), "Account name cannot be null");

        if (string.IsNullOrEmpty(account.Name)) throw new ArgumentException("Account name cannot be null or empty");

        return base.UpdateAsync(account);
    }

    public async Task<IEnumerable<Account>> GetByBrokerIdAsync(Guid brokerId)
    {
        if (brokerId == null) throw new ArgumentNullException(nameof(brokerId), "Broker ID cannot be null");

        var accounts = await FindAsync(a => a.BrokerId == brokerId);
        _logger.LogDebug("Retrieved accounts {@Accounts} from DbSet for broker Id {BrokerId}", accounts, brokerId);
        return accounts;
    }

    public async Task<decimal> GetBrokerBalance(Guid brokerId)
    {
        if (brokerId == null) throw new ArgumentNullException(nameof(brokerId), "Broker ID cannot be null");

        var accounts = await GetByBrokerIdAsync(brokerId);
        var balance = accounts.Sum(a => a.Balance);
        _logger.LogDebug("Retrieved broker balance {Balance:C} from accounts {@Accounts} for broker Id {BrokerId}",
            balance, accounts, brokerId);
        return balance;
    }
}