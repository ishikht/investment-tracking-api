using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;
using InvestmentTracking.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvestmentTracking.Data.Tests.Repositories;

public class BrokerRepositoryTests
{
    private readonly SqlDbContext _dbContext;
    private readonly IBrokerRepository _repository;

    public BrokerRepositoryTests()
    {
        // Set up the test environment
        var options = new DbContextOptionsBuilder<SqlDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var mockLogger = new Mock<ILogger<BrokerRepository>>();
        _dbContext = new SqlDbContext(options);
        _repository = new BrokerRepository(_dbContext, mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_AddsBrokerToDatabase()
    {
        // Arrange
        var broker = new Broker { Name = "Test Broker" };

        // Act
        await _repository.AddAsync(broker);
        await _dbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(1, _dbContext.Brokers.Count());

        var addedBroker = await _repository.GetAsync(broker.Id);
        Assert.NotNull(addedBroker);
        Assert.Equal(broker.Name, addedBroker.Name);
        Assert.NotEqual(Guid.Empty, addedBroker.Id);
    }

    [Fact]
    public async Task GetAsync_GetsBrokerById()
    {
        // Arrange
        var broker = new Broker { Name = "Test Broker" };
        await _repository.AddAsync(broker);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(broker.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(broker.Id, result.Id);
        Assert.Equal(broker.Name, result.Name);
    }

    [Fact]
    public async Task AddAsync_DoesNotChangeIdWhenItIsSet()
    {
        // Arrange
        var broker = new Broker { Id = Guid.NewGuid(), Name = "Test Broker" };

        // Act
        await _repository.AddAsync(broker);
        await _dbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(1, _dbContext.Brokers.Count());

        var addedBroker = await _repository.GetAsync(broker.Id);
        Assert.NotNull(addedBroker);
        Assert.Equal(broker.Id, addedBroker.Id);
        Assert.Equal(broker.Name, addedBroker.Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBrokerInDatabase()
    {
        // Arrange
        var broker = new Broker { Name = "Test Broker" };
        await _repository.AddAsync(broker);
        await _dbContext.SaveChangesAsync();
        var updatedBroker = new Broker { Id = broker.Id, Name = "Updated Broker" };

        // Act
        await _repository.UpdateAsync(updatedBroker);
        await _dbContext.SaveChangesAsync();

        // Assert
        var result = await _dbContext.Brokers.FindAsync(broker.Id);
        Assert.Equal(updatedBroker.Name, result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsExceptionWhenBrokerNotFound()
    {
        // Arrange
        var broker = new Broker { Id = Guid.NewGuid(), Name = "Test Broker" };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.UpdateAsync(broker));
    }

    [Fact]
    public async Task DeleteAsync_DeletesBrokerFromDatabase()
    {
        // Arrange
        var broker = new Broker { Name = "Test Broker" };
        await _repository.AddAsync(broker);
        await _dbContext.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(broker);
        await _dbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(0, _dbContext.Brokers.Count());
    }
}