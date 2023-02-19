using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;
using InvestmentTracking.Core.Services;
using Microsoft.Extensions.Logging;

namespace InvestmentTracking.Services;

public class TransactionService : ITransactionService
{
    private readonly ILogger<TransactionService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(IUnitOfWork unitOfWork, ILogger<TransactionService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Transaction> AddTransactionAsync(Transaction transaction)
    {
        await _unitOfWork.TransactionRepository.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Added transaction {@Transaction} to database", transaction);
        return transaction;
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        var transactions = await _unitOfWork.TransactionRepository.GetAllAsync();
        _logger.LogInformation("Retrieved all transactions from database");
        return transactions;
    }

    public async Task<Transaction?> GetTransactionByIdAsync(Guid id)
    {
        var transaction = await _unitOfWork.TransactionRepository.GetAsync(id);
        if (transaction != null)
            _logger.LogInformation("Retrieved transaction {@Transaction} from database by Id {Id}", transaction, id);
        else
            _logger.LogWarning("No transaction found in database with Id {Id}", id);
        return transaction;
    }

    public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
    {
        await _unitOfWork.TransactionRepository.UpdateAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Updated transaction {@Transaction} in database", transaction);
        return transaction;
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

    public async Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId)
    {
        var transactions = await _unitOfWork.TransactionRepository.GetByAccountIdAsync(accountId);
        _logger.LogInformation("Retrieved transactions {@Transactions} from database for account Id {AccountId}",
            transactions,
            accountId);
        return transactions;
    }
}