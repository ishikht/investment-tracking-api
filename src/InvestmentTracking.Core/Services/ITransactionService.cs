using InvestmentTracking.Core.Entities;

namespace InvestmentTracking.Core.Services;

public interface ITransactionService
{
    Task<Transaction> AddTransactionAsync(Transaction transaction);
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    Task<Transaction?> GetTransactionByIdAsync(Guid id);
    Task<Transaction> UpdateTransactionAsync(Transaction transaction);
    Task DeleteTransactionAsync(Guid id);
    Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId);
}