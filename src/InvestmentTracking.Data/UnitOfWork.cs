using InvestmentTracking.Core.Data;

namespace InvestmentTracking.Data;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly SqlDbContext _context;

    public UnitOfWork(SqlDbContext context,
        IBrokerRepository brokerRepository,
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository)
    {
        // Throw an exception if any of the dependencies are null
        _context = context ?? throw new ArgumentNullException(nameof(context));
        BrokerRepository = brokerRepository ?? throw new ArgumentNullException(nameof(brokerRepository));
        AccountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        TransactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
    }

    public IBrokerRepository BrokerRepository { get; }

    public IAccountRepository AccountRepository { get; }

    public ITransactionRepository TransactionRepository { get; }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
