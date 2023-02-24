using AutoMapper;
using InvestmentTracking.Core;
using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvestmentTracking.Services.Tests;

[Collection("AutoMapper")]
public class TransactionServiceTests
{
    private readonly Mock<ILogger<TransactionService>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly TransactionFactory _transactionFactory;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public TransactionServiceTests(AutoMapperFixture fixture)
    {
        _mapper = fixture.Mapper;
        _transactionFactory = new TransactionFactory(_mapper);

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _unitOfWorkMock.Setup(uow => uow.TransactionRepository).Returns(_transactionRepositoryMock.Object);

        _loggerMock = new Mock<ILogger<TransactionService>>();
    }

    [Fact]
    public async Task AddTransactionAsync_Should_AddNewTransactionToDatabase()
    {
        // Arrange
        var stockTransactionDto = new StockTransactionDto
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            Ticker = "AAPL",
            Date = DateTime.Now,
            Shares = 10,
            Amount = 1000,
            Commission = 10
        };

        Transaction capturedTransaction = null;


        _transactionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Transaction>()))
            .Returns((Transaction t) =>
            {
                capturedTransaction = t;
                capturedTransaction.Id = t.Id;
                return Task.FromResult(capturedTransaction);
            });

        var transactionService =
            new TransactionService(_unitOfWorkMock.Object, _mapper, _transactionFactory, _loggerMock.Object);

        // Act
        var result = await transactionService.AddTransactionAsync(stockTransactionDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(stockTransactionDto.Id, result.Id);
        Assert.Equal(stockTransactionDto.AccountId, result.AccountId);
        Assert.Equal(stockTransactionDto.Ticker, ((StockTransactionDto) result).Ticker);
        Assert.Equal(stockTransactionDto.Date, result.Date);
        Assert.Equal(stockTransactionDto.Shares, ((StockTransactionDto) result).Shares);
        Assert.Equal(stockTransactionDto.Amount, result.Amount);
        Assert.Equal(stockTransactionDto.Commission, result.Commission);

        _unitOfWorkMock.Verify(x => x.TransactionRepository.AddAsync(It.IsAny<Transaction>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        Assert.NotNull(capturedTransaction);
        Assert.Equal(stockTransactionDto.AccountId, capturedTransaction.AccountId);
        Assert.Equal(stockTransactionDto.Ticker, ((StockTransaction) capturedTransaction).Ticker);
        Assert.Equal(stockTransactionDto.Date, capturedTransaction.Date);
        Assert.Equal(stockTransactionDto.Shares, ((StockTransaction) capturedTransaction).Shares);
        Assert.Equal(stockTransactionDto.Amount, capturedTransaction.Amount);
        Assert.Equal(stockTransactionDto.Commission, capturedTransaction.Commission);
    }

    [Fact]
    public async Task GetAllTransactionsAsync_ShouldReturnAllTransactions()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new StockTransaction {Id = Guid.NewGuid(), Ticker = "AAPL", Shares = 10, Amount = 100.00M},
            new AccountTransaction {Id = Guid.NewGuid(), AccountId = Guid.NewGuid(), Amount = 500.00M},
            new IncomeTransaction {Id = Guid.NewGuid(), Amount = 5000.00M}
        };
        _unitOfWorkMock.Setup(uow => uow.TransactionRepository.GetAllAsync()).Returns(transactions.ToAsyncEnumerable());

        // Act
        var result = new List<TransactionDto>();
        var transactionService =
            new TransactionService(_unitOfWorkMock.Object, _mapper, _transactionFactory, _loggerMock.Object);
        await foreach (var transactionDto in transactionService.GetAllTransactionsAsync()) result.Add(transactionDto);

        // Assert
        Assert.Equal(transactions.Count, result.Count);
        for (var i = 0; i < transactions.Count; i++) Assert.Equal(transactions[i].Id, result[i].Id);
    }

    [Theory]
    [InlineData(typeof(StockTransaction), "AAPL", 10, 100.00)]
    [InlineData(typeof(AccountTransaction), null, null, 500.00)]
    [InlineData(typeof(IncomeTransaction), null, null, 5000.00)]
    public async Task GetTransactionByIdAsync_ReturnsTransaction_IfExists(Type transactionType, string ticker, int? shares, decimal amount)
    {
        // Arrange
        var transactionId = Guid.NewGuid();

        Transaction transaction = null;
        if (transactionType == typeof(StockTransaction))
        {
            transaction = new StockTransaction { Id = transactionId, Ticker = ticker, Shares = shares.Value, Amount = amount };
        }
        else if (transactionType == typeof(AccountTransaction))
        {
            transaction = new AccountTransaction { Id = transactionId, AccountId = Guid.NewGuid(), Amount = amount };
        }
        else if (transactionType == typeof(IncomeTransaction))
        {
            transaction = new IncomeTransaction { Id = transactionId, Amount = amount };
        }


        _transactionRepositoryMock.Setup(x => x.GetAsync(transactionId)).ReturnsAsync(transaction);

        _unitOfWorkMock.Setup(x => x.TransactionRepository).Returns(_transactionRepositoryMock.Object);

        var transactionService =
            new TransactionService(_unitOfWorkMock.Object, _mapper, _transactionFactory, _loggerMock.Object);

        // Act
        var result = await transactionService.GetTransactionByIdAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.Id);
        Assert.Equal(transactionType.Name.Replace("Transaction", "TransactionDto"), result.GetType().Name);

        if (transactionType == typeof(StockTransaction))
        {
            Assert.Equal(ticker, ((StockTransactionDto)result).Ticker);
            Assert.Equal(shares, ((StockTransactionDto)result).Shares);
        }

        Assert.Equal(amount, result.Amount);

        _unitOfWorkMock.Verify(x => x.TransactionRepository.GetAsync(transactionId), Times.Once);
    }

    [Fact]
    public async Task UpdateTransactionAsync_Should_UpdateExistingTransactionInDatabase()
    {
        // Arrange
        var accountTransactionDto = new AccountTransactionDto
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Amount = 1000
        };
        var accountTransaction = _mapper.Map<AccountTransaction>(accountTransactionDto);


        _transactionRepositoryMock.Setup(x => x.GetAsync(accountTransactionDto.Id)).ReturnsAsync(accountTransaction);

        _unitOfWorkMock.Setup(x => x.TransactionRepository).Returns(_transactionRepositoryMock.Object);

        var transactionService =
            new TransactionService(_unitOfWorkMock.Object, _mapper, _transactionFactory, _loggerMock.Object);

        // Act
        accountTransactionDto.Amount = 500;
        await transactionService.UpdateTransactionAsync(accountTransactionDto);

        // Assert
        _unitOfWorkMock.Verify(x => x.TransactionRepository.GetAsync(accountTransactionDto.Id), Times.Once);
        _unitOfWorkMock.Verify(x => x.TransactionRepository.UpdateAsync(It.IsAny<AccountTransaction>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteTransactionAsync_Should_DeleteExistingTransactionFromDatabase()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var accountTransactionDto = new AccountTransactionDto
        {
            Id = transactionId,
            AccountId = Guid.NewGuid(),
            Date = DateTime.Now,
            Amount = 500
        };
        var accountTransaction = _mapper.Map<AccountTransaction>(accountTransactionDto);


        _transactionRepositoryMock.Setup(x => x.GetAsync(transactionId)).ReturnsAsync(accountTransaction);

        _unitOfWorkMock.Setup(x => x.TransactionRepository).Returns(_transactionRepositoryMock.Object);

        var transactionService =
            new TransactionService(_unitOfWorkMock.Object, _mapper, _transactionFactory, _loggerMock.Object);

        // Act
        await transactionService.DeleteTransactionAsync(transactionId);

        // Assert
        _unitOfWorkMock.Verify(x => x.TransactionRepository.GetAsync(transactionId), Times.Once);
        _unitOfWorkMock.Verify(x => x.TransactionRepository.DeleteAsync(accountTransaction), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTransactionsByAccountIdAsync_ReturnsTransactions_ByAccountId()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transactions = new List<Transaction>
        {
            new StockTransaction
            {
                Id = Guid.NewGuid(), AccountId = accountId, Ticker = "AAPL", Date = DateTime.UtcNow, Shares = 10,
                Amount = 1000, Commission = 10
            },
            new AccountTransaction {Id = Guid.NewGuid(), AccountId = accountId, Date = DateTime.UtcNow, Amount = 500},
            new IncomeTransaction
                {Id = Guid.NewGuid(), AccountId = Guid.NewGuid(), Date = DateTime.UtcNow, Amount = 1000}
        };
        var expectedTransactionDtos = transactions
            .Where(t => t.AccountId == accountId)
            .Select<Transaction, TransactionDto>(t =>
            {
                switch (t)
                {
                    case StockTransaction stockTransaction:
                        return _mapper.Map<StockTransactionDto>(stockTransaction);
                    case AccountTransaction accountTransaction:
                        return _mapper.Map<AccountTransactionDto>(accountTransaction);
                    case IncomeTransaction incomeTransaction:
                        return _mapper.Map<IncomeTransactionDto>(incomeTransaction);
                    default:
                        throw new InvalidOperationException($"Unrecognized transaction type: {t.GetType().Name}");
                }
            })
            .ToList();


        _transactionRepositoryMock
            .Setup(x => x.GetByAccountIdAsync(accountId))
            .ReturnsAsync((Guid id) => { return transactions.Where(t => t.AccountId == id); });

        _unitOfWorkMock.Setup(x => x.TransactionRepository).Returns(_transactionRepositoryMock.Object);

        var transactionService =
            new TransactionService(_unitOfWorkMock.Object, _mapper, _transactionFactory, _loggerMock.Object);

        // Act
        var result = await transactionService.GetTransactionsByAccountIdAsync(accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTransactionDtos.Count, result.Count());

        foreach (var transactionDto in expectedTransactionDtos)
            Assert.Contains(result,
                t => t.Id == transactionDto.Id && t.AccountId == transactionDto.AccountId &&
                     t.Amount == transactionDto.Amount && t.Date == transactionDto.Date);

        _unitOfWorkMock.Verify(x => x.TransactionRepository.GetByAccountIdAsync(accountId), Times.Once);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_WithInvalidTransactionId_ReturnsNull()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        Transaction nullTransaction = null;

        _transactionRepositoryMock.Setup(x => x.GetAsync(transactionId)).ReturnsAsync(nullTransaction);

        _unitOfWorkMock.Setup(x => x.TransactionRepository).Returns(_transactionRepositoryMock.Object);

        var transactionService =
            new TransactionService(_unitOfWorkMock.Object, _mapper, _transactionFactory, _loggerMock.Object);

        // Act
        var result = await transactionService.GetTransactionByIdAsync(transactionId);

        // Assert
        Assert.Null(result);
        _unitOfWorkMock.Verify(x => x.TransactionRepository.GetAsync(transactionId), Times.Once);
    }

    [Fact]
    public async Task UpdateTransactionAsync_WithInvalidTransactionId_ThrowsException()
    {
        // Arrange
        var invalidTransactionId = Guid.NewGuid();
        var accountTransactionDto = new AccountTransactionDto
        {
            Id = invalidTransactionId,
            AccountId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Amount = 1000
        };
        Transaction nullTransaction = null;

        _transactionRepositoryMock.Setup(x => x.GetAsync(invalidTransactionId)).ReturnsAsync(nullTransaction);

        _unitOfWorkMock.Setup(x => x.TransactionRepository).Returns(_transactionRepositoryMock.Object);

        var transactionService =
            new TransactionService(_unitOfWorkMock.Object, _mapper, _transactionFactory, _loggerMock.Object);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            transactionService.UpdateTransactionAsync(accountTransactionDto));

        _unitOfWorkMock.Verify(x => x.TransactionRepository.GetAsync(invalidTransactionId), Times.Once);
    }
}