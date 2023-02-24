using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;
using InvestmentTracking.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvestmentTracking.Data.Tests.Repositories;

public class AccountRepositoryTests
{
    private readonly SqlDbContext _dbContext;
    private readonly IAccountRepository _repository;

    public AccountRepositoryTests()
    {
        // Set up the test environment
        var options = new DbContextOptionsBuilder<SqlDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var mockLogger = new Mock<ILogger<AccountRepository>>();
        _dbContext = new SqlDbContext(options);
        _repository = new AccountRepository(_dbContext, mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_GeneratesNewId_WhenIdIsEmpty()
    {
        // Arrange
        var entity = new Account { Name = "Test Account" };
        entity.Id = Guid.Empty;

        // Act
        await _repository.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        // Assert
        Assert.NotEqual(Guid.Empty, entity.Id);
    }

    [Fact]
    public async Task AddAsync_AddsAccountToDatabase()
    {
        // Arrange
        var account = new Account { Name = "Test Account", BrokerId = Guid.NewGuid() };

        // Act
        await _repository.AddAsync(account);
        await _dbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(1, _dbContext.Accounts.Count());
        var result = await _dbContext.Accounts.FindAsync(account.Id);
        Assert.NotNull(result);
        Assert.Equal(account.Name, result.Name);
        Assert.Equal(account.BrokerId, result.BrokerId);
    }

    [Fact]
    public async Task AddAsync_ThrowsExceptionWhenNameIsNull()
    {
        // Arrange
        var account = new Account { BrokerId = Guid.NewGuid() };

        // Act and assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddAsync(account));
    }

    [Fact]
    public async Task AddAsync_ThrowsExceptionWhenNameIsEmpty()
    {
        // Arrange
        var account = new Account { Name = "", BrokerId = Guid.NewGuid() };

        // Act and assert
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.AddAsync(account));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesAccountInDatabase()
    {
        // Arrange
        var account = new Account { Name = "Test Account", BrokerId = Guid.NewGuid() };
        await _repository.AddAsync(account);
        await _dbContext.SaveChangesAsync();

        var updatedAccount = new Account { Id = account.Id, Name = "Updated Account", BrokerId = account.BrokerId };

        // Act
        await _repository.UpdateAsync(updatedAccount);
        await _dbContext.SaveChangesAsync();

        // Assert
        Account result = await _dbContext.Accounts.FindAsync(account.Id);
        Assert.Equal(updatedAccount.Name, result.Name);
        Assert.Equal(updatedAccount.BrokerId, result.BrokerId);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsExceptionWhenNameIsNull()
    {
        // Arrange
        var account = new Account { Name = "Test Account", BrokerId = Guid.NewGuid() };
        await _repository.AddAsync(account);
        await _dbContext.SaveChangesAsync();

        var updatedAccount = new Account { Id = account.Id, BrokerId = account.BrokerId };

        // Act and assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateAsync(updatedAccount));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsExceptionWhenNameIsEmpty()
    {
        // Arrange
        var account = new Account { Name = "Test Account", BrokerId = Guid.NewGuid() };
        await _repository.AddAsync(account);
        await _dbContext.SaveChangesAsync();

        var updatedAccount = new Account { Id = account.Id, Name = "", BrokerId = account.BrokerId };

        // Act and assert
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.UpdateAsync(updatedAccount));
    }

    [Fact]
    public async Task GetByBrokerIdAsync_ReturnsAccountsForBroker()
    {
        // Arrange
        var brokerId = Guid.NewGuid();
        var accounts = new List<Account>
        {
            new Account { Name = "Account 1", BrokerId = brokerId },
            new Account { Name = "Account 2", BrokerId = brokerId },
            new Account { Name = "Account 3", BrokerId = Guid.NewGuid() }
        };
        await _repository.AddRangeAsync(accounts);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByBrokerIdAsync(brokerId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, account => Assert.Equal(brokerId, account.BrokerId));
    }
}