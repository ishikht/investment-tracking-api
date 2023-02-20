using InvestmentTracking.Core.Entities;
using InvestmentTracking.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentTracking.Api.Controllers;

[ApiController]
[Route("[controller]")]
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Account>> AddAccount([FromBody] Account account)
    {
        try
        {
            var addedAccount = await _accountService.AddAccountAsync(account);
            return CreatedAtAction(nameof(GetAccountById), new {id = addedAccount.Id}, addedAccount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding account to database");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAccounts()
    {
        try
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accounts from database");
            return BadRequest();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetAccountById(Guid id)
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
            _logger.LogError(ex, "Error retrieving account from database");
            return BadRequest();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] Account account)
    {
        if (id != account.Id) return BadRequest();

        try
        {
            await _accountService.UpdateAccountAsync(account);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account in database");
            return BadRequest();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        try
        {
            await _accountService.DeleteAccountAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account from database");
            return BadRequest();
        }
    }

    [HttpGet("broker/{brokerId}")]
    public async Task<ActionResult> GetAccountsByBrokerId(Guid brokerId)
    {
        try
        {
            var accounts = await _accountService.GetAccountsByBrokerIdAsync(brokerId);
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accounts from database for broker Id {BrokerId}", brokerId);
            return BadRequest();
        }
    }

    [HttpGet("broker/{brokerId}/balance")]
    public async Task<ActionResult> GetBrokerBalance(Guid brokerId)
    {
        try
        {
            var balance = await _accountService.GetBrokerBalanceAsync(brokerId);
            return Ok(new {Balance = balance});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving broker balance for Id {BrokerId}", brokerId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while retrieving broker balance.");
        }
    }
}