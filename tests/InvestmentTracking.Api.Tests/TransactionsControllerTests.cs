using InvestmentTracking.Api.Controllers;
using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvestmentTracking.Api.Tests;

public class TransactionsControllerTests
{
    private readonly TransactionsController _controller;
    private readonly Mock<ILogger<TransactionsController>> _mockLogger;
    private readonly Mock<ITransactionService> _mockTransactionService;

    public TransactionsControllerTests()
    {
        _mockTransactionService = new Mock<ITransactionService>();
        _mockLogger = new Mock<ILogger<TransactionsController>>();
        _controller = new TransactionsController(_mockTransactionService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task AddTransaction_ReturnsCreated_WithValidDto()
    {
        // Arrange
        var transactionDto = new TransactionDto
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            Amount = 1000.00M,
            Date = DateTime.UtcNow
        };

        var addedTransaction = new TransactionDto
        {
            Id = transactionDto.Id,
            AccountId = transactionDto.AccountId,
            Amount = transactionDto.Amount,
            Date = transactionDto.Date
        };

        _mockTransactionService.Setup(x => x.AddTransactionAsync(It.IsAny<TransactionDto>()))
            .ReturnsAsync(addedTransaction);

        // Act
        var result = await _controller.AddTransaction(transactionDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var transactionResult = Assert.IsType<TransactionDto>(createdAtActionResult.Value);
        Assert.Equal(transactionDto.Id, transactionResult.Id);
        Assert.Equal(transactionDto.AccountId, transactionResult.AccountId);
        Assert.Equal(transactionDto.Amount, transactionResult.Amount);
        Assert.Equal(transactionDto.Date, transactionResult.Date);
    }

    [Fact]
    public async Task AddTransaction_ReturnsBadRequest_WithInvalidDto()
    {
        // Arrange
        var transactionDto = new TransactionDto
        {
            // Missing required properties
        };

        _controller.ModelState.AddModelError("Error", "Invalid transaction DTO");

        // Act
        var result = await _controller.AddTransaction(transactionDto);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task AddTransaction_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var transactionDto = new TransactionDto
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            Amount = 1000.00M,
            Date = DateTime.UtcNow
        };

        _mockTransactionService.Setup(x => x.AddTransactionAsync(It.IsAny<TransactionDto>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.AddTransaction(transactionDto);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task GetAllTransactions_ReturnsListOfTransactions()
    {
        // Arrange
        var transactions = new List<TransactionDto>
        {
            new TransactionDto
            {
                Id = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                Amount = 1000.00M,
                Date = DateTime.UtcNow
            },
            new TransactionDto
            {
                Id = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                Amount = 500.00M,
                Date = DateTime.UtcNow
            }
        };

        _mockTransactionService.Setup(x => x.GetAllTransactionsAsync())
            .Returns(transactions.ToAsyncEnumerable());

        // Act
        var result = await _controller.GetAllTransactions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var transactionResult = Assert.IsAssignableFrom<IEnumerable<TransactionDto>>(okResult.Value);
        Assert.Equal(transactions.Count, transactionResult.Count());
    }
    [Fact]
    public async Task GetAllTransactions_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        _mockTransactionService.Setup(x => x.GetAllTransactionsAsync())
            .Throws(new Exception("Test Exception"));

        // Act
        var result = await _controller.GetAllTransactions();

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTransactionById_ReturnsTransaction_WithValidId()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var transactionDto = new TransactionDto
        {
            Id = transactionId,
            AccountId = Guid.NewGuid(),
            Amount = 1000.00M,
            Date = DateTime.UtcNow
        };

        _mockTransactionService.Setup(x => x.GetTransactionByIdAsync(transactionId))
            .ReturnsAsync(transactionDto);

        // Act
        var result = await _controller.GetTransactionById(transactionId);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
        var transactionResult = Assert.IsType<TransactionDto>(okObjectResult.Value);
        Assert.Equal(transactionDto.Id, transactionResult.Id);
        Assert.Equal(transactionDto.AccountId, transactionResult.AccountId);
        Assert.Equal(transactionDto.Amount, transactionResult.Amount);
        Assert.Equal(transactionDto.Date, transactionResult.Date);
    }

    [Fact]
    public async Task GetTransactionById_ReturnsNotFound_WithInvalidId()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        TransactionDto nullTransaction = null;
        _mockTransactionService.Setup(x => x.GetTransactionByIdAsync(invalidId))
            .ReturnsAsync(nullTransaction);

        // Act
        var result = await _controller.GetTransactionById(invalidId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task GetTransactionById_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        _mockTransactionService.Setup(x => x.GetTransactionByIdAsync(invalidId))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetTransactionById(invalidId);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task UpdateTransaction_ReturnsNoContent_WithValidId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var transactionDto = new TransactionDto
        {
            Id = id,
            AccountId = Guid.NewGuid(),
            Amount = 2000.00M,
            Date = DateTime.UtcNow
        };

        _mockTransactionService.Setup(x => x.UpdateTransactionAsync(It.IsAny<TransactionDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateTransaction(id, transactionDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateTransaction_ReturnsBadRequest_WithInvalidId()
    {
        // Arrange
        var transactionDto = new TransactionDto
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            Amount = 1000.00M,
            Date = DateTime.UtcNow
        };

        // Act
        var result = await _controller.UpdateTransaction(Guid.Empty, transactionDto);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpdateTransaction_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var transactionDto = new TransactionDto
        {
            Id = id,
            AccountId = Guid.NewGuid(),
            Amount = 1000.00M,
            Date = DateTime.UtcNow
        };

        _mockTransactionService.Setup(x => x.UpdateTransactionAsync(It.IsAny<TransactionDto>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.UpdateTransaction(id, transactionDto);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteTransaction_ReturnsNoContent_WithValidId()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        _mockTransactionService.Setup(x => x.DeleteTransactionAsync(transactionId)).Verifiable();

        // Act
        var result = await _controller.DeleteTransaction(transactionId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockTransactionService.Verify(x => x.DeleteTransactionAsync(transactionId), Times.Once);
    }

    [Fact]
    public async Task DeleteTransaction_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        _mockTransactionService.Setup(x => x.DeleteTransactionAsync(transactionId))
            .Throws(new Exception("Error deleting transaction"));

        // Act
        var result = await _controller.DeleteTransaction(transactionId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        _mockLogger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => true),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task GetTransactionsByAccountId_ReturnsListOfTransactions_WithValidAccountId()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var expectedTransactions = new List<TransactionDto>
        {
            new TransactionDto
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                Amount = 1000.00M,
                Date = DateTime.UtcNow
            },
            new TransactionDto
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                Amount = 2000.00M,
                Date = DateTime.UtcNow
            }
        };

        _mockTransactionService.Setup(x => x.GetTransactionsByAccountIdAsync(accountId))
            .ReturnsAsync(expectedTransactions);

        // Act
        var result = await _controller.GetTransactionsByAccountId(accountId);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
        var transactionDtos = Assert.IsAssignableFrom<IEnumerable<TransactionDto>>(okObjectResult.Value);
        Assert.Equal(expectedTransactions.Count, transactionDtos.Count());
    }
    [Fact]
    public async Task GetTransactionsByAccountId_ReturnsBadRequest_WithInvalidAccountId()
    {
        // Arrange
        var invalidId = Guid.Empty;

        // Act
        var result = await _controller.GetTransactionsByAccountId(invalidId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task GetTransactionsByAccountId_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        _mockTransactionService.Setup(x => x.GetTransactionsByAccountIdAsync(accountId))
            .ThrowsAsync(new Exception("Test Exception"));

        // Act
        var result = await _controller.GetTransactionsByAccountId(accountId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

}