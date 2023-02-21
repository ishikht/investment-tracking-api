using InvestmentTracking.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestmentTracking.Data.Tests;

public class SqlDbContextTests
{
    [Fact]
    public void CanCreateSqlDbContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlDbContext>()
            .UseInMemoryDatabase("CanCreateSqlDbContext")
            .Options;

        // Act
        using (var context = new SqlDbContext(options))
        {
            // Assert
            Assert.NotNull(context);
        }
    }

    [Fact]
    public void CanAddAndReadEntitiesFromDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlDbContext>()
            .UseInMemoryDatabase("CanAddAndReadEntitiesFromDatabase")
            .Options;

        var account = new Account {Name = "Test Account", BrokerId = Guid.NewGuid()};

        // Act
        using (var context = new SqlDbContext(options))
        {
            context.Accounts.Add(account);
            context.SaveChanges();
        }

        // Assert
        using (var context = new SqlDbContext(options))
        {
            var result = context.Accounts.Find(account.Id);
            Assert.NotNull(result);
            Assert.Equal(account.Name, result.Name);
            Assert.Equal(account.BrokerId, result.BrokerId);
        }
    }
}