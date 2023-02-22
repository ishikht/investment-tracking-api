using InvestmentTracking.Core.Dtos;

public interface IAccountService
{
    Task<AccountDto> AddAccountAsync(AccountDto accountDto);
    IAsyncEnumerable<AccountDto> GetAllAccountsAsync();
    Task<AccountDto?> GetAccountByIdAsync(Guid id);
    Task UpdateAccountAsync(AccountDto accountDto);
    Task DeleteAccountAsync(Guid id);
    Task<IEnumerable<AccountDto>> GetAccountsByBrokerIdAsync(Guid brokerId);
    Task<decimal> GetBrokerBalanceAsync(Guid brokerId);
}