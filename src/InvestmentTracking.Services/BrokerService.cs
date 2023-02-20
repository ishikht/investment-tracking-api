using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;
using InvestmentTracking.Core.Services;
using Microsoft.Extensions.Logging;

namespace InvestmentTracking.Services;

public class BrokerService : IBrokerService
{
    private readonly ILogger<BrokerService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public BrokerService(IUnitOfWork unitOfWork, ILogger<BrokerService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Broker> AddBrokerAsync(Broker broker)
    {
        await _unitOfWork.BrokerRepository.AddAsync(broker);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Added broker {@Broker} to database", broker);
        return broker;
    }

    public async Task<IEnumerable<Broker>> GetAllBrokersAsync()
    {
        var brokers = await _unitOfWork.BrokerRepository.GetAllAsync();
        _logger.LogInformation("Retrieved all brokers from database");
        return brokers;
    }

    public async Task<Broker> GetBrokerByIdAsync(Guid id)
    {
        var broker = await _unitOfWork.BrokerRepository.GetAsync(id);
        if (broker != null)
            _logger.LogInformation("Retrieved broker {@Broker} from database by Id {Id}", broker, id);
        else
            _logger.LogWarning("No broker found in database with Id {Id}", id);
        return broker;
    }

    public async Task<Broker> UpdateBrokerAsync(Broker broker)
    {
        await _unitOfWork.BrokerRepository.UpdateAsync(broker);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Updated broker {@Broker} in database", broker);
        return broker;
    }

    public async Task DeleteBrokerAsync(Guid id)
    {
        var broker = await _unitOfWork.BrokerRepository.GetAsync(id);
        if (broker == null)
        {
            _logger.LogWarning("No broker found in database with Id {Id} to delete", id);
            return;
        }

        await _unitOfWork.BrokerRepository.DeleteAsync(broker);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Deleted broker {@Broker} from database", broker);
    }
}