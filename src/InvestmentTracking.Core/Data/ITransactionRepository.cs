using InvestmentTracking.Core.Entities;

namespace InvestmentTracking.Core.Data;

/// <summary>
/// Interface for the transaction repository.
/// </summary>
public interface ITransactionRepository : IRepository<Transaction>
{
    /// <summary>
    /// Gets all transactions belonging to the specified account.
    /// </summary>
    /// <param name="accountId">The ID of the account.</param>
    /// <returns>A collection of transactions belonging to the specified account.</returns>
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId);
}