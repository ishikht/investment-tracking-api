using InvestmentTracking.Core.Entities;

namespace InvestmentTracking.Core.Data;

/// <summary>
/// Interface for the account repository.
/// </summary>
public interface IAccountRepository : IRepository<Account>
{
    /// <summary>
    /// Gets all accounts belonging to the specified broker.
    /// </summary>
    /// <param name="brokerId">The ID of the broker.</param>
    /// <returns>A collection of accounts belonging to the specified broker.</returns>
    Task<IEnumerable<Account>> GetByBrokerIdAsync(Guid brokerId);

    /// <summary>
    /// Gets the total balance of all accounts belonging to the specified broker.
    /// </summary>
    /// <param name="brokerId">The ID of the broker.</param>
    /// <returns>The total balance of all accounts belonging to the specified broker.</returns>
    Task<decimal> GetBrokerBalance(Guid brokerId);
}