using AutoMapper;
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
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TransactionService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<TransactionDto> AddTransactionAsync(TransactionDto transactionDto)
    {
        var transaction = _mapper.Map<Transaction>(transactionDto);
        await _unitOfWork.TransactionRepository.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Added transaction {@Transaction} to database", transaction);

        var addedTransactionDto = _mapper.Map<TransactionDto>(transaction);
        return addedTransactionDto;
    }

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
    {
        var transactions = await _unitOfWork.TransactionRepository.GetAllAsync();
        _logger.LogInformation("Retrieved all transactions from database");

        var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        return transactionDtos;
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

        var transactionDto = _mapper.Map<TransactionDto>(transaction);
        return transactionDto;
    }

    public async Task UpdateTransactionAsync(TransactionDto transactionDto)
    {
        var transaction = await _unitOfWork.TransactionRepository.GetAsync(transactionDto.Id);
        if (transaction == null)
        {
            _logger.LogWarning("No transaction found in database with Id {Id} to update", transactionDto.Id);
            return;
        }

        _mapper.Map(transactionDto, transaction);
        await _unitOfWork.TransactionRepository.UpdateAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Updated transaction {@Transaction} in database", transaction);
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
        _logger.LogInformation("Retrieved transactions {@Transactions} from database for account Id {AccountId}", transactions, accountId);

        var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        return transactionDtos;
    }
}