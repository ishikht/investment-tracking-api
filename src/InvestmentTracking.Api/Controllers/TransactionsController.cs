using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentTracking.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TransactionDto>> AddTransaction([FromBody] TransactionDto transactionDto)
    {
        try
        {
            var addedTransaction = await _transactionService.AddTransactionAsync(transactionDto);
            return CreatedAtAction(nameof(GetTransactionById), new { id = addedTransaction.Id }, addedTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding transaction to database");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllTransactions()
    {
        try
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions from database");
            return BadRequest();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetTransactionById(Guid id)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction != null)
                return Ok(transaction);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction from database");
            return BadRequest();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] TransactionDto transactionDto)
    {
        if (id != transactionDto.Id) return BadRequest();

        try
        {
            await _transactionService.UpdateTransactionAsync(transactionDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction in database");
            return BadRequest();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        try
        {
            await _transactionService.DeleteTransactionAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transaction from database");
            return BadRequest();
        }
    }

    [HttpGet("account/{accountId}")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByAccountId(Guid accountId)
    {
        try
        {
            var transactions = await _transactionService.GetTransactionsByAccountIdAsync(accountId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions from database for account Id {AccountId}", accountId);
            return BadRequest();
        }
    }
}