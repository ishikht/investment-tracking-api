using AutoMapper;
using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvestmentTracking.Services.Tests;

[Collection("AutoMapper")]
public class AccountServiceTests
{
    private readonly Mock<ILogger<AccountService>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public AccountServiceTests(AutoMapperFixture fixture)
    {
        _mapper = fixture.Mapper;

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock.Setup(uow => uow.AccountRepository).Returns(accountRepositoryMock.Object);

        _loggerMock = new Mock<ILogger<AccountService>>();
    }

    [Fact]
    public async Task AddAccountAsync_Should_AddNewAccountToDatabase()
    {
        // Arrange
        var accountDto = new AccountCreateDto()
        {
            Name = "Test Account",
            BrokerId = Guid.NewGuid()
        };

        Account savedAccount = null;

        var accountRepositoryMock = new Mock<IAccountRepository>();
        accountRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Account>()))
            .Returns((Account a) =>
            {
                savedAccount = a;
                savedAccount.Id = a.Id;
                return Task.FromResult(savedAccount);
            });

        _unitOfWorkMock.Setup(x => x.AccountRepository).Returns(accountRepositoryMock.Object);

        var accountService = new AccountService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);

        // Act
        var result = await accountService.AddAccountAsync(accountDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.NewGuid(), result.Id);
        Assert.Equal(accountDto.Name, result.Name);
        Assert.Equal(accountDto.BrokerId, result.BrokerId);

        _unitOfWorkMock.Verify(x => x.AccountRepository.AddAsync(It.IsAny<Account>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        Assert.NotNull(savedAccount);
        Assert.Equal(accountDto.Name, savedAccount.Name);
        Assert.Equal(accountDto.BrokerId, savedAccount.BrokerId);
    }

    [Fact]
    public async Task GetAllAccountsAsync_ReturnsAllAccounts()
    {
        // Arrange
        var accounts = new List<Account>
        {
            new() {Id = Guid.NewGuid(), Name = "Account 1", BrokerId = Guid.NewGuid()},
            new() {Id = Guid.NewGuid(), Name = "Account 2", BrokerId = Guid.NewGuid()}
        };

        var accountRepositoryMock = new Mock<IAccountRepository>();
        accountRepositoryMock.Setup(x => x.GetAllAsync()).Returns(accounts.ToAsyncEnumerable());
        _unitOfWorkMock.Setup(x => x.AccountRepository).Returns(accountRepositoryMock.Object);
        var service = new AccountService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetAllAccountsAsync().ToListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Collection(result,
            item => Assert.Equal(accounts[0].Id, item.Id),
            item => Assert.Equal(accounts[1].Id, item.Id)
        );
    }

    [Fact]
    public async Task GetAccountByIdAsync_ReturnsAccount_IfExists()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new Account { Id = accountId, Name = "Test Account", BrokerId = Guid.NewGuid() };

        var accountRepositoryMock = new Mock<IAccountRepository>();
        accountRepositoryMock.Setup(x => x.GetAsync(accountId)).ReturnsAsync(account);
        _unitOfWorkMock.Setup(x => x.AccountRepository).Returns(accountRepositoryMock.Object);
        var service = new AccountService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetAccountByIdAsync(accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accountId, result.Id);
        Assert.Equal(account.Name, result.Name);
        Assert.Equal(account.BrokerId, result.BrokerId);
    }

    [Fact]
    public async Task UpdateAccountAsync_Should_UpdateExistingAccountInDatabase()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var originalAccount = new Account { Id = accountId, Name = "Test Account", BrokerId = Guid.NewGuid() };
        var updatedAccountDto = new AccountUpdateDto { Name = "Updated Test Account" };

        var accountRepositoryMock = new Mock<IAccountRepository>();
        accountRepositoryMock.Setup(x => x.GetAsync(accountId)).ReturnsAsync(originalAccount);

        Account savedAccount = null;
        accountRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Account>()))
            .Returns((Account a) =>
            {
                savedAccount = a;
                return Task.CompletedTask;
            });

        _unitOfWorkMock.Setup(x => x.AccountRepository).Returns(accountRepositoryMock.Object);

        var accountService = new AccountService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);

        // Act
        await accountService.UpdateAccountAsync(accountId, updatedAccountDto);

        // Assert
        accountRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Account>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        Assert.NotNull(savedAccount);
        Assert.Equal(updatedAccountDto.Name, savedAccount.Name);
    }

    [Fact]
    public async Task DeleteAccountAsync_Should_DeleteExistingAccountFromDatabase()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new Account { Id = accountId, Name = "Test Account", BrokerId = Guid.NewGuid() };

        var accountRepositoryMock = new Mock<IAccountRepository>();
        accountRepositoryMock.Setup(x => x.GetAsync(accountId)).ReturnsAsync(account);
        accountRepositoryMock.Setup(x => x.DeleteAsync(account)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.AccountRepository).Returns(accountRepositoryMock.Object);

        var accountService = new AccountService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);

        // Act
        await accountService.DeleteAccountAsync(accountId);

        // Assert
        accountRepositoryMock.Verify(x => x.DeleteAsync(account), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAccountsByBrokerIdAsync_ReturnsAccounts_ByBrokerId()
    {
        // Arrange
        var brokerId = Guid.NewGuid();
        var accounts = new List<Account>
        {
            new() {Id = Guid.NewGuid(), Name = "Account 1", BrokerId = brokerId},
            new() {Id = Guid.NewGuid(), Name = "Account 2", BrokerId = Guid.NewGuid()},
            new() {Id = Guid.NewGuid(), Name = "Account 3", BrokerId = brokerId},
        };

        var accountRepositoryMock = new Mock<IAccountRepository>();
        accountRepositoryMock.Setup(x => x.GetByBrokerIdAsync(brokerId)).ReturnsAsync(accounts.Where(a => a.BrokerId == brokerId));
        _unitOfWorkMock.Setup(x => x.AccountRepository).Returns(accountRepositoryMock.Object);
        var service = new AccountService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);

        // Act
        var result = await service.GetAccountsByBrokerIdAsync(brokerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Collection(result,
            item => Assert.Equal(accounts[0].Id, item.Id),
            item => Assert.Equal(accounts[2].Id, item.Id)
        );
    }

}