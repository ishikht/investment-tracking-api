using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Entities;
using InvestmentTracking.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentTracking.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class BrokersController : ControllerBase
{
    private readonly IBrokerService _brokerService;
    private readonly ILogger<BrokersController> _logger;

    public BrokersController(IBrokerService brokerService, ILogger<BrokersController> logger)
    {
        _brokerService = brokerService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(BrokerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBroker([FromBody] BrokerDto broker)
    {
        try
        {
            var createdBroker = await _brokerService.AddBrokerAsync(broker);
            return CreatedAtAction(nameof(GetBroker), new {id = createdBroker.Id}, createdBroker);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating broker");
            return BadRequest();
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BrokerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBrokers()
    {
        try
        {
            var brokers = await _brokerService.GetAllBrokersAsync();
            return Ok(brokers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving brokers");
            return BadRequest();
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BrokerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBroker(Guid id)
    {
        try
        {
            var broker = await _brokerService.GetBrokerByIdAsync(id);
            if (broker != null)
                return Ok(broker);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving broker by Id {Id}", id);
            return BadRequest();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BrokerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBroker(Guid id, [FromBody] BrokerDto broker)
    {
        if (id != broker.Id)
            return BadRequest();
        
        if (broker == null) return BadRequest();

        try
        {
            await _brokerService.UpdateBrokerAsync(broker);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating broker with Id {Id}", id);
            return BadRequest();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBroker(Guid id)
    {
        try
        {
            await _brokerService.DeleteBrokerAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting broker with Id {Id}", id);
            return NotFound();
        }
    }
}