using AutoMapper;
using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Entities;
using Microsoft.Extensions.Logging;

namespace InvestmentTracking.Services;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AccountService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<AccountDto> AddAccountAsync(AccountDto accountDto)
    {
        var account = _mapper.Map<Account>(accountDto);
        await _unitOfWork.AccountRepository.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Added account {@Account} to database", account);

        var addedAccountDto = _mapper.Map<AccountDto>(account);
        return addedAccountDto;
    }

    public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync()
    {
        var accounts = await _unitOfWork.AccountRepository.GetAllAsync();
        _logger.LogInformation("Retrieved all accounts from database");

        var accountDtos = _mapper.Map<IEnumerable<AccountDto>>(accounts);
        return accountDtos;
    }

    public async Task<AccountDto?> GetAccountByIdAsync(Guid id)
    {
        var account = await _unitOfWork.AccountRepository.GetAsync(id);
        if (account != null)
        {
            _logger.LogInformation("Retrieved account {@Account} from database by Id {Id}", account, id);
        }
        else
        {
            _logger.LogWarning("No account found in database with Id {Id}", id);
            return null;
        }

        var accountDto = _mapper.Map<AccountDto>(account);
        return accountDto;
    }

    public async Task UpdateAccountAsync(AccountDto accountDto)
    {
        var account = await _unitOfWork.AccountRepository.GetAsync(accountDto.Id);
        if (account == null)
        {
            _logger.LogWarning("No account found in database with Id {Id} to update", accountDto.Id);
            return;
        }

        _mapper.Map(accountDto, account);
        await _unitOfWork.AccountRepository.UpdateAsync(account);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Updated account {@Account} in database", account);
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

    public async Task<IEnumerable<AccountDto>> GetAccountsByBrokerIdAsync(Guid brokerId)
    {
        var accounts = await _unitOfWork.AccountRepository.GetByBrokerIdAsync(brokerId);
        _logger.LogInformation("Retrieved all accounts from database for broker Id {Id}", brokerId);

        var accountDtos = _mapper.Map<IEnumerable<AccountDto>>(accounts);
        return accountDtos;
    }

    public async Task<decimal> GetBrokerBalanceAsync(Guid brokerId)
    {
        var balance = await _unitOfWork.AccountRepository.GetBrokerBalance(brokerId);
        _logger.LogInformation("Retrieved broker balance {Balance:C} from accounts for broker Id {Id}", balance,
            brokerId);

        return balance;
    }
}