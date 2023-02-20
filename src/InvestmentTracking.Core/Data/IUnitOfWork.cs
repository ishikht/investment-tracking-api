namespace InvestmentTracking.Core.Data;

/// <summary>
/// Unit of work interface responsible for coordinating database interactions between repositories.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets the broker repository.
    /// </summary>
    IBrokerRepository BrokerRepository { get; }

    /// <summary>
    /// Gets the account repository.
    /// </summary>
    IAccountRepository AccountRepository { get; }

    /// <summary>
    /// Gets the transaction repository.
    /// </summary>
    ITransactionRepository TransactionRepository { get; }

    /// <summary>
    /// Saves changes to the database.
    /// </summary>
    void SaveChanges();

    /// <summary>
    /// Saves changes to the database asynchronously.
    /// </summary>
    Task SaveChangesAsync();
}