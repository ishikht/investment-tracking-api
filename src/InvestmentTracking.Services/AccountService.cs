using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;
using InvestmentTracking.Core.Services;
using Microsoft.Extensions.Logging;

namespace InvestmentTracking.Services;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Account> AddAccountAsync(Account account)
    {
        await _unitOfWork.AccountRepository.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Added account {@Account} to database", account);
        return account;
    }

    public async Task<IEnumerable<Account>> GetAllAccountsAsync()
    {
        var accounts = await _unitOfWork.AccountRepository.GetAllAsync();
        _logger.LogInformation("Retrieved all accounts from database");
        return accounts;
    }

    public async Task<Account> GetAccountByIdAsync(Guid id)
    {
        var account = await _unitOfWork.AccountRepository.GetAsync(id);
        if (account != null)
            _logger.LogInformation("Retrieved account {@Account} from database by Id {Id}", account, id);
        else
            _logger.LogWarning("No account found in database with Id {Id}", id);
        return account;
    }

    public async Task<Account> UpdateAccountAsync(Account account)
    {
        await _unitOfWork.AccountRepository.UpdateAsync(account);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Updated account {@Account} in database", account);
        return account;
    }

    public async Task DeleteAccountAsync(Guid id)
    {
        var account = await _unitOfWork.AccountRepository.GetAsync(id);
        if (account == null)
        {
            _logger.LogWarning("No account found in database with Id {Id} to delete", id);
            return;
        }

        await _unitOfWork.AccountRepository.DeleteAsync(account);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Deleted account {@Account} from database", account);
    }

    public async Task<IEnumerable<Account>> GetAccountsByBrokerIdAsync(Guid brokerId)
    {
        var accounts = await _unitOfWork.AccountRepository.GetByBrokerIdAsync(brokerId);
        _logger.LogInformation("Retrieved accounts {@Accounts} from database for broker Id {BrokerId}", accounts,
            brokerId);
        return accounts;
    }

    public async Task<decimal> GetBrokerBalanceAsync(Guid brokerId)
    {
        var accounts = await GetAccountsByBrokerIdAsync(brokerId);
        var balance = accounts.Sum(a => a.Balance);
        _logger.LogInformation(
            "Retrieved broker balance {Balance:C} from accounts {@Accounts} for broker Id {BrokerId}", balance,
            accounts, brokerId);
        return balance;
    }
}