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
        _context = context;
        BrokerRepository = brokerRepository;
        AccountRepository = accountRepository;
        TransactionRepository = transactionRepository;
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
