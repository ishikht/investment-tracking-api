using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;
using InvestmentTracking.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvestmentTracking.Data.Tests.Repositories;

public class TransactionRepositoryTests
{
    private readonly SqlDbContext _dbContext;
    private readonly ITransactionRepository _repository;

    public TransactionRepositoryTests()
    {
        // Set up the test environment
        var options = new DbContextOptionsBuilder<SqlDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var mockLogger = new Mock<ILogger<TransactionRepository>>();
        _dbContext = new SqlDbContext(options);
        _repository = new TransactionRepository(_dbContext, mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_AddsTransactionToDatabase()
    {
        // Arrange
        var account = new Account { Name = "Test Account" };
        var transaction = new StockTransaction
        {
            Ticker = "AAPL",
            Shares = 10,
            Amount = 1000,
            Commission = 10,
            Date = DateTime.Now,
            Account = account
        };
        await _dbContext.Accounts.AddAsync(account);
        await _dbContext.SaveChangesAsync();

        // Act
        await _repository.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();

        // Assert
        var result = await _dbContext.Transactions.FindAsync(transaction.Id);
        Assert.NotNull(result);
        Assert.IsType<StockTransaction>(result);
        Assert.Equal(transaction.Ticker, ((StockTransaction)result).Ticker);
        Assert.Equal(transaction.Shares, ((StockTransaction)result).Shares);
        Assert.Equal(transaction.Amount, result.Amount);
        Assert.Equal(transaction.Commission, result.Commission);
        Assert.Equal(transaction.Date, result.Date);
        Assert.Equal(account.Id, result.AccountId);
    }

    [Fact]
    public async Task GetAsync_ReturnsTransactionById()
    {
        // Arrange
        var account = new Account { Name = "Test Account" };
        var transaction = new StockTransaction
        {
            Ticker = "AAPL",
            Shares = 10,
            Amount = 1000,
            Commission = 10,
            Date = DateTime.Now,
            Account = account
        };
        await _repository.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(transaction.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transaction.Id, result.Id);
        Assert.IsType<StockTransaction>(result);
        Assert.Equal(transaction.Ticker, ((StockTransaction)result).Ticker);
        Assert.Equal(transaction.Shares, ((StockTransaction)result).Shares);
        Assert.Equal(transaction.Amount, result.Amount);
        Assert.Equal(transaction.Commission, result.Commission);
        Assert.Equal(transaction.Date, result.Date);
        Assert.NotNull(result.Account);
        Assert.Equal(account.Id, result.Account.Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTransactions()
    {
        // Arrange
        var account = new Account { Name = "Test Account" };
        var transactions = new List<Transaction>
        {
            new StockTransaction { Amount = 1000, Commission = 10, Date = DateTime.Now, Ticker = "AAPL", Shares = 10, Account = account },
            new AccountTransaction { Amount = 500, Commission = 5, Date = DateTime.Now, Account = account },
            new IncomeTransaction { Amount = 200, Commission = 0, Date = DateTime.Now, Account = account }
        };
        foreach (var transaction in transactions)
        {
            await _repository.AddAsync(transaction);
        }
        await _dbContext.SaveChangesAsync();

        // Act
        int count = 0;
        var result =  _repository.GetAllAsync();
        await foreach (var item in result)
        {
            count++;
        }

        // Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task GetByAccountIdAsync_ReturnsTransactionsForAccount()
    {
        // Arrange
        var account = new Account { Name = "Test Account" };
        await _dbContext.Accounts.AddAsync(account);
        await _dbContext.SaveChangesAsync();

        var transactions = new List<Transaction>
        {
            new StockTransaction { Ticker = "AAPL", Shares = 10, Amount = 1000, Commission = 10, Date = DateTime.Now, AccountId = account.Id },
            new AccountTransaction { Amount = 500, Commission = 5, Date = DateTime.Now.AddDays(-1), AccountId = account.Id },
            new IncomeTransaction { Amount = 100, Date = DateTime.Now.AddDays(-2), AccountId = account.Id }
        };
        await _dbContext.Transactions.AddRangeAsync(transactions);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByAccountIdAsync(account.Id);

        // Assert
        Assert.Equal(transactions.Count, result.Count());
        Assert.True(result.All(t => t.AccountId == account.Id));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTransactionInDatabase()
    {
        // Arrange
        var transaction = new StockTransaction { Ticker = "ABC", Shares = 100, Date = DateTime.Now, Amount = 1000.0M, Commission = 10.0M, AccountId = Guid.NewGuid() };
        await _repository.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();

        var updatedTransaction = new StockTransaction { Id = transaction.Id, Ticker = "DEF", Shares = 200, Date = DateTime.Now.AddDays(-1), Amount = 2000.0M, Commission = 20.0M, AccountId = transaction.AccountId };

        // Act
        await _repository.UpdateAsync(updatedTransaction);
        await _dbContext.SaveChangesAsync();

        // Assert
        StockTransaction result = (StockTransaction) await _dbContext.Transactions.FindAsync(transaction.Id);
        Assert.Equal(updatedTransaction.Ticker, result.Ticker);
        Assert.Equal(updatedTransaction.Shares, result.Shares);
        Assert.Equal(updatedTransaction.Date, result.Date);
        Assert.Equal(updatedTransaction.Amount, result.Amount);
        Assert.Equal(updatedTransaction.Commission, result.Commission);
        Assert.Equal(updatedTransaction.AccountId, result.AccountId);
    }

    [Fact]
    public async Task DeleteAsync_DeletesTransactionFromDatabase()
    {
        // Arrange
        var account = new Account { Name = "Test Account" };
        var transaction = new StockTransaction { Ticker = "AAPL", Shares = 10, Amount = 100, Commission = 1.0M, Account = account, Date = DateTime.Now };
        await _dbContext.Transactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(transaction);
        await _dbContext.SaveChangesAsync();

        // Assert
        var result = await _dbContext.Transactions.FindAsync(transaction.Id);
        Assert.Null(result);
    }
}