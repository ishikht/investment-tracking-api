using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;
using InvestmentTracking.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvestmentTracking.Data.Tests;

public class UnitOfWorkTests
{
    private readonly SqlDbContext _dbContext;
    private readonly IBrokerRepository _brokerRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    public UnitOfWorkTests()
    {
        // Set up the test environment
        var options = new DbContextOptionsBuilder<SqlDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new SqlDbContext(options);

        _brokerRepository = new BrokerRepository(context, new Mock<ILogger<BrokerRepository>>().Object);
        _accountRepository = new AccountRepository(context, new Mock<ILogger<AccountRepository>>().Object);
        _transactionRepository = new TransactionRepository(context, new Mock<ILogger<TransactionRepository>>().Object);

        _dbContext = context;

        UnitOfWork = new UnitOfWork(context, _brokerRepository, _accountRepository, _transactionRepository);
    }

    public UnitOfWork UnitOfWork { get; }

    [Fact]
    public void Constructor_ThrowsException_WhenSqlDbContextIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new UnitOfWork(null, _brokerRepository, _accountRepository, _transactionRepository));
    }

    [Fact]
    public void Constructor_ThrowsException_WhenBrokerRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new UnitOfWork(_dbContext, null, _accountRepository, _transactionRepository));
    }

    [Fact]
    public void Constructor_ThrowsException_WhenAccountRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new UnitOfWork(_dbContext, _brokerRepository, null, _transactionRepository));
    }

    [Fact]
    public void Constructor_ThrowsException_WhenTransactionRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new UnitOfWork(_dbContext, _brokerRepository, _accountRepository, null));
    }

    [Fact]
    public async Task SaveChanges_CommitsChangesToTheDatabase()
    {
        // Arrange
        var broker = new Broker { Name = "Test Broker" };
        await UnitOfWork.BrokerRepository.AddAsync(broker);

        // Act
        UnitOfWork.SaveChanges();

        // Assert
        var result = _dbContext.Brokers.FirstOrDefault(b => b.Id == broker.Id);
        Assert.NotNull(result);
        Assert.Equal(broker.Name, result.Name);
    }

    [Fact]
    public async Task SaveChangesAsync_CommitsChangesToTheDatabase()
    {
        // Arrange
        var account = new Account { Name = "Test Account", BrokerId = Guid.NewGuid() };
        await UnitOfWork.AccountRepository.AddAsync(account);

        // Act
        await UnitOfWork.SaveChangesAsync();

        // Assert
        var result = await _dbContext.Accounts.FindAsync(account.Id);
        Assert.NotNull(result);
        Assert.Equal(account.Name, result.Name);
        Assert.Equal(account.BrokerId, result.BrokerId);
    }
}