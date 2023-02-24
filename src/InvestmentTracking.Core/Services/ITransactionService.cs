using InvestmentTracking.Core.Dtos;

namespace InvestmentTracking.Core.Services;

public interface ITransactionService
{
    Task<TransactionDto> AddTransactionAsync(TransactionDto transactionDto);
    IAsyncEnumerable<TransactionDto> GetAllTransactionsAsync();
    Task<TransactionDto?> GetTransactionByIdAsync(Guid id);
    Task UpdateTransactionAsync(TransactionDto transactionDto);
    Task DeleteTransactionAsync(Guid id);
    Task<IEnumerable<TransactionDto>> GetTransactionsByAccountIdAsync(Guid accountId);
}