using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;
using Microsoft.Extensions.Logging;

namespace InvestmentTracking.Data.Repositories;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    private readonly ILogger<TransactionRepository> _logger;

    public TransactionRepository(SqlDbContext dbContext, ILogger<TransactionRepository> logger) : base(dbContext, logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId)
    {
        var transactions = await FindAsync(t => t.AccountId == accountId);
        _logger.LogDebug("Retrieved transactions {@Transactions} from DbSet for account Id {AccountId}", transactions, accountId);
        return transactions;
    }
}