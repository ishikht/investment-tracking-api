using AutoMapper;
using InvestmentTracking.Core;
using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Entities;
using InvestmentTracking.Core.Services;
using Microsoft.Extensions.Logging;

namespace InvestmentTracking.Services;

public class TransactionService : ITransactionService
{
    private readonly ILogger<TransactionService> _logger;
    private readonly IMapper _mapper;
    private readonly ITransactionFactory _transactionFactory;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(IUnitOfWork unitOfWork,
        IMapper mapper,
        ITransactionFactory transactionFactory,
        ILogger<TransactionService> logger)
    {
        _transactionFactory = transactionFactory;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<TransactionDto> AddTransactionAsync(TransactionDto transactionDto)
    {
        var transaction = _transactionFactory.CreateTransaction(transactionDto);

        await _unitOfWork.TransactionRepository.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Added transaction {@Transaction} to database", transaction);

        // Determine the corresponding derived type of TransactionDto and use that type for mapping
        var addedTransactionDto = _transactionFactory.CreateTransactionDto(transaction);
        return addedTransactionDto;
    }

    public async IAsyncEnumerable<TransactionDto> GetAllTransactionsAsync()
    {
        var transactions = _unitOfWork.TransactionRepository.GetAllAsync();

        await foreach (var transaction in transactions)
            yield return _transactionFactory.CreateTransactionDto(transaction);

        _logger.LogInformation("Retrieved all transactions from database");
    }

    public async Task<TransactionDto?> GetTransactionByIdAsync(Guid id)
    {
        var transaction = await _unitOfWork.TransactionRepository.GetAsync(id);
        if (transaction != null)
        {
            _logger.LogInformation("Retrieved transaction {@Transaction} from database by Id {Id}", transaction, id);
        }
        else
        {
            _logger.LogWarning("No transaction found in database with Id {Id}", id);
            return null;
        }

        var transactionDto = _transactionFactory.CreateTransactionDto(transaction);
        return transactionDto;
    }


    public async Task UpdateTransactionAsync(TransactionDto transactionDto)
    {
        if (transactionDto == null || transactionDto.Id == Guid.Empty)
        {
            _logger.LogWarning("Transaction ID cannot be null or empty");
            throw new ArgumentException("Transaction ID cannot be null or empty");
        }

        var transaction = await _unitOfWork.TransactionRepository.GetAsync(transactionDto.Id);
        if (transaction == null)
        {
            _logger.LogWarning("Transaction with ID {TransactionId} not found.", transactionDto.Id);
            throw new ArgumentException($"Transaction with ID {transactionDto.Id} not found.");
        }

        switch (transaction)
        {
            case StockTransaction stockTransaction:
                _mapper.Map((StockTransactionDto) transactionDto, stockTransaction);
                break;
            case AccountTransaction accountTransaction:
                _mapper.Map((AccountTransactionDto) transactionDto, accountTransaction);
                break;
            case IncomeTransaction incomeTransaction:
                _mapper.Map((IncomeTransactionDto) transactionDto, incomeTransaction);
                break;
            default:
                _logger.LogError("Unrecognized transaction type: {TransactionType}", transaction.GetType().Name);
                throw new InvalidOperationException($"Unrecognized transaction type: {transaction.GetType().Name}");
        }

        await _unitOfWork.TransactionRepository.UpdateAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Transaction with ID {TransactionId} updated successfully.", transactionDto.Id);
    }

    public async Task DeleteTransactionAsync(Guid id)
    {
        var transaction = await _unitOfWork.TransactionRepository.GetAsync(id);
        if (transaction == null)
        {
            _logger.LogWarning("No transaction found in database with Id {Id} to delete", id);
            return;
        }

        await _unitOfWork.TransactionRepository.DeleteAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Deleted transaction {@Transaction} from database", transaction);
    }

    public async Task<IEnumerable<TransactionDto>> GetTransactionsByAccountIdAsync(Guid accountId)
    {
        var transactions = await _unitOfWork.TransactionRepository.GetByAccountIdAsync(accountId);
        _logger.LogInformation("Retrieved transactions {@Transactions} from database for account Id {AccountId}",
            transactions, accountId);

        var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        return transactionDtos;
    }
}