using InvestmentTracking.Core.Entities;

namespace InvestmentTracking.Core.Services;

public interface IAccountService
{
    Task<Account> AddAccountAsync(Account account);
    Task<IEnumerable<Account>> GetAllAccountsAsync();
    Task<Account> GetAccountByIdAsync(Guid id);
    Task<Account> UpdateAccountAsync(Account account);
    Task DeleteAccountAsync(Guid id);
    Task<IEnumerable<Account>> GetAccountsByBrokerIdAsync(Guid brokerId);
    Task<decimal> GetBrokerBalanceAsync(Guid brokerId);
}