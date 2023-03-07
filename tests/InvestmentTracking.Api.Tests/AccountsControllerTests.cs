using InvestmentTracking.Api.Controllers;
using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InvestmentTracking.Api.Tests;

public class AccountsControllerTests
{
    private readonly AccountsController _accountsController;
    private readonly Mock<IAccountService> _mockAccountService;

    public AccountsControllerTests()
    {
        _mockAccountService = new Mock<IAccountService>();
        _accountsController =
            new AccountsController(_mockAccountService.Object, NullLogger<AccountsController>.Instance);
    }

    [Fact]
    public async Task CreateAccount_ReturnsCreated_WithValidDto()
    {
        // Arrange
        var accountDto = new AccountCreateDto
        {
            Name = "Test Account",
            BrokerId = Guid.NewGuid()
        };

        var createdAccount = new AccountDto
        {
            Id = Guid.NewGuid(),
            Name = accountDto.Name,
            BrokerId = accountDto.BrokerId
        };

        _mockAccountService.Setup(service => service.AddAccountAsync(accountDto)).ReturnsAsync(createdAccount);

        // Act
        var result = await _accountsController.CreateAccount(accountDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var resultValue = Assert.IsType<AccountDto>(createdResult.Value);
        Assert.Equal(createdAccount.Id, resultValue.Id);
        Assert.Equal(createdAccount.Name, resultValue.Name);
        Assert.Equal(createdAccount.BrokerId, resultValue.BrokerId);
        Assert.Equal(nameof(AccountsController.GetAccount), createdResult.ActionName);
    }

    [Fact]
    public async Task CreateAccount_ReturnsBadRequest_WithInvalidDto()
    {
        // Arrange
        var mockAccountService = new Mock<IAccountService>();
        _accountsController.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _accountsController.CreateAccount(new AccountCreateDto());

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task CreateAccount_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var accountDto = new AccountCreateDto
        {
            Name = "Test Account",
            BrokerId = Guid.NewGuid()
        };

        _mockAccountService.Setup(service => service.AddAccountAsync(accountDto)).Throws(new Exception());

        // Act
        var result = await _accountsController.CreateAccount(accountDto);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetAccounts_ReturnsListOfAccounts()
    {
        // Arrange
        var accounts = new List<AccountDto>
        {
            new AccountDto { Id = Guid.NewGuid(), Name = "Account 1", BrokerId = Guid.NewGuid() },
            new AccountDto { Id = Guid.NewGuid(), Name = "Account 2", BrokerId = Guid.NewGuid() }
        };
        _mockAccountService.Setup(service => service.GetAllAccountsAsync()).Returns(accounts.ToAsyncEnumerable());

        // Act
        var result = await _accountsController.GetAccounts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var resultValue = Assert.IsType<List<AccountDto>>(okResult.Value);
        Assert.Equal(accounts.Count, resultValue.Count);
    }

    [Fact]
    public async Task GetAccounts_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var mockAccountService = new Mock<IAccountService>();
        mockAccountService.Setup(service => service.GetAllAccountsAsync()).Throws(new Exception());
        var controller = new AccountsController(mockAccountService.Object, NullLogger<AccountsController>.Instance);

        // Act
        var result = await controller.GetAccounts();

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task GetAccount_ReturnsAccount_WithValidId()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new AccountDto
        {
            Id = accountId,
            Name = "Test Account",
            BrokerId = Guid.NewGuid()
        };
        _mockAccountService.Setup(service => service.GetAccountByIdAsync(accountId)).ReturnsAsync(account);

        // Act
        var result = await _accountsController.GetAccount(accountId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultValue = Assert.IsType<AccountDto>(okResult.Value);
        Assert.Equal(account.Id, resultValue.Id);
        Assert.Equal(account.Name, resultValue.Name);
        Assert.Equal(account.BrokerId, resultValue.BrokerId);
    }

    [Fact]
    public async Task GetAccount_ReturnsNotFound_WithInvalidId()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        _mockAccountService.Setup(service => service.GetAccountByIdAsync(invalidId)).ReturnsAsync((AccountDto)null);

        // Act
        var result = await _accountsController.GetAccount(invalidId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task GetAccount_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var mockAccountService = new Mock<IAccountService>();
        mockAccountService.Setup(service => service.GetAccountByIdAsync(It.IsAny<Guid>())).Throws(new Exception());
        var controller = new AccountsController(mockAccountService.Object, NullLogger<AccountsController>.Instance);

        // Act
        var result = await controller.GetAccount(Guid.NewGuid());

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
    [Fact]
    public async Task UpdateAccount_ReturnsOk_WithValidId()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var accountDto = new AccountUpdateDto { Name = "Test Account"};
        _mockAccountService.Setup(service => service.UpdateAccountAsync(accountId,accountDto)).Verifiable();

        // Act
        var result = await _accountsController.UpdateAccount(accountId, accountDto);

        // Assert
        _mockAccountService.Verify();
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateAccount_ReturnsBadRequest_WithInvalidId()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var accountDto = new AccountUpdateDto { Name = "Test Account" };
        var mockAccountService = new Mock<IAccountService>();
        mockAccountService.Setup(service => service.UpdateAccountAsync(It.IsAny<Guid>(), It.IsAny<AccountUpdateDto>())).Throws<KeyNotFoundException>();
        var controller = new AccountsController(mockAccountService.Object, NullLogger<AccountsController>.Instance);

        // Act
        var result = await controller.UpdateAccount(accountId, accountDto);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpdateAccount_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var accountDto = new AccountUpdateDto
        {
            Name = "Test Account",
        };

        _mockAccountService.Setup(service => service.UpdateAccountAsync(id, accountDto))
            .Throws(new Exception());

        // Act
        var result = await _accountsController.UpdateAccount(id, accountDto);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteAccount_ReturnsNoContent_WithValidId()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        // Act
        var result = await _accountsController.DeleteAccount(accountId);

        // Assert
        _mockAccountService.Verify(x => x.DeleteAccountAsync(accountId), Times.Once);

        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, noContentResult.StatusCode);
    }

    [Fact]
    public async Task DeleteAccount_ReturnsNotFound_WithInvalidId()
    {
        // Arrange
        var invalidAccountId = Guid.NewGuid();
        _mockAccountService.Setup(service => service.DeleteAccountAsync(invalidAccountId)).ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _accountsController.DeleteAccount(invalidAccountId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAccountsByBrokerId_ReturnsListOfAccounts()
    {
        // Arrange
        var brokerId = Guid.NewGuid();
        var accounts = new List<AccountDto>
        {
            new AccountDto { Id = Guid.NewGuid(), Name = "Account 1", BrokerId = brokerId },
            new AccountDto { Id = Guid.NewGuid(), Name = "Account 2", BrokerId = brokerId },
            new AccountDto { Id = Guid.NewGuid(), Name = "Account 3", BrokerId = brokerId }
        };
        _mockAccountService.Setup(service => service.GetAccountsByBrokerIdAsync(brokerId)).ReturnsAsync(accounts);

        // Act
        var result = await _accountsController.GetAccountsByBrokerId(brokerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultValue = Assert.IsType<List<AccountDto>>(okResult.Value);
        Assert.Equal(accounts.Count, resultValue.Count);
        Assert.Equal(accounts[0].Id, resultValue[0].Id);
        Assert.Equal(accounts[0].Name, resultValue[0].Name);
        Assert.Equal(accounts[0].BrokerId, resultValue[0].BrokerId);
        Assert.Equal(accounts[1].Id, resultValue[1].Id);
        Assert.Equal(accounts[1].Name, resultValue[1].Name);
        Assert.Equal(accounts[1].BrokerId, resultValue[1].BrokerId);
        Assert.Equal(accounts[2].Id, resultValue[2].Id);
        Assert.Equal(accounts[2].Name, resultValue[2].Name);
        Assert.Equal(accounts[2].BrokerId, resultValue[2].BrokerId);
    }

    [Fact]
    public async Task GetAccountsByBrokerId_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var brokerId = Guid.NewGuid();
        _mockAccountService.Setup(service => service.GetAccountsByBrokerIdAsync(brokerId)).ThrowsAsync(new Exception());

        // Act
        var result = await _accountsController.GetAccountsByBrokerId(brokerId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetBrokerBalance_ReturnsBalance_WithValidBrokerId()
    {
        // Arrange
        var brokerId = Guid.NewGuid();
        var balance = 1000m;

        _mockAccountService.Setup(service => service.GetBrokerBalanceAsync(brokerId)).ReturnsAsync(balance);

        // Act
        var result = await _accountsController.GetBrokerBalance(brokerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(balance, okResult.Value);
    }

    [Fact]
    public async Task GetBrokerBalance_ReturnsBadRequest_WithInvalidBrokerId()
    {
        // Arrange
        var invalidBrokerId = Guid.Empty;

        // Act
        var result = await _accountsController.GetBrokerBalance(invalidBrokerId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetBrokerBalance_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var invalidBrokerId = Guid.NewGuid();
        _mockAccountService.Setup(service => service.GetBrokerBalanceAsync(invalidBrokerId))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _accountsController.GetBrokerBalance(invalidBrokerId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

}