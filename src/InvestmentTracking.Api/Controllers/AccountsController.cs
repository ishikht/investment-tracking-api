using InvestmentTracking.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentTracking.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAccount([FromBody] AccountDto accountDto)
    {
        try
        {
            var createdAccount = await _accountService.AddAccountAsync(accountDto);
            return CreatedAtAction(nameof(GetAccount), new { id = createdAccount.Id }, createdAccount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            return BadRequest();
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccounts()
    {
        try
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accounts");
            return BadRequest();
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccount(Guid id)
    {
        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account != null)
                return Ok(account);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account by Id {Id}", id);
            return BadRequest();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] AccountDto accountDto)
    {
        if (id != accountDto.Id)
            return BadRequest();

        try
        {
            await _accountService.UpdateAccountAsync(accountDto);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account with Id {Id}", id);
            return BadRequest();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        try
        {
            await _accountService.DeleteAccountAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account with Id {Id}", id);
            return NotFound();
        }
    }

    [HttpGet("broker/{brokerId}")]
    [ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccountsByBrokerId(Guid brokerId)
    {
        try
        {
            var accounts = await _accountService.GetAccountsByBrokerIdAsync(brokerId);
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accounts for broker with Id {BrokerId}", brokerId);
            return BadRequest();
        }
    }

    [HttpGet("broker/{brokerId}/balance")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBrokerBalance(Guid brokerId)
    {
        try
        {
            var balance = await _accountService.GetBrokerBalanceAsync(brokerId);
            return Ok(balance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving balance for broker with Id {BrokerId}", brokerId);
            return BadRequest();
        }
    }
}