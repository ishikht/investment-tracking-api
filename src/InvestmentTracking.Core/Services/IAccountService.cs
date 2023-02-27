using InvestmentTracking.Core.Dtos;

public interface IAccountService
{
    Task<AccountDto> AddAccountAsync(AccountCreateDto accountDto);
    IAsyncEnumerable<AccountDto> GetAllAccountsAsync();
    Task<AccountDto?> GetAccountByIdAsync(Guid id);
    Task UpdateAccountAsync(Guid id, AccountUpdateDto accountDto);
    Task DeleteAccountAsync(Guid id);
    Task<IEnumerable<AccountDto>> GetAccountsByBrokerIdAsync(Guid brokerId);
    Task<decimal> GetBrokerBalanceAsync(Guid brokerId);
}