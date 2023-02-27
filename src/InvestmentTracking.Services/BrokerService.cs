using AutoMapper;
using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Entities;
using InvestmentTracking.Core.Services;
using Microsoft.Extensions.Logging;

namespace InvestmentTracking.Services;

public class BrokerService : IBrokerService
{
    private readonly ILogger<BrokerService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BrokerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BrokerService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<BrokerDto> AddBrokerAsync(BrokerDto brokerDto)
    {
        var broker = _mapper.Map<Broker>(brokerDto);
        await _unitOfWork.BrokerRepository.AddAsync(broker);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Added broker {@Broker} to database", broker);

        brokerDto = _mapper.Map<BrokerDto>(broker);
        return brokerDto;
    }

    public async IAsyncEnumerable<BrokerDto> GetAllBrokersAsync()
    {
        var brokers =  _unitOfWork.BrokerRepository.GetAllAsync();
        await foreach (var broker in brokers)
        {
            yield return _mapper.Map<BrokerDto>(broker);
        }
        _logger.LogInformation("Retrieved all brokers from database");
    }

    public async Task<BrokerDto?> GetBrokerByIdAsync(Guid id)
    {
        var broker = await _unitOfWork.BrokerRepository.GetAsync(id);
        if (broker != null)
        {
            _logger.LogInformation("Retrieved broker {@Broker} from database by Id {Id}", broker, id);
        }
        else
        {
            _logger.LogWarning("No broker found in database with Id {Id}", id);
            return null;
        }
        var brokerDto = _mapper.Map<BrokerDto>(broker);
        return brokerDto;
    }

    public async Task UpdateBrokerAsync(Guid id, BrokerUpdateDto brokerDto)
    {
        var broker = await _unitOfWork.BrokerRepository.GetAsync(id);
        if (broker == null)
        {
            _logger.LogWarning("No broker found in database with Id {Id} to update", id);
            throw new KeyNotFoundException($"No broker found in database with Id {id} to update");
        }
        _mapper.Map(brokerDto, broker);
        await _unitOfWork.BrokerRepository.UpdateAsync(broker);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Updated broker {@Broker} in database", broker);
    }

    public async Task DeleteBrokerAsync(Guid id)
    {
        var broker = await _unitOfWork.BrokerRepository.GetAsync(id);
        if (broker == null)
        {
            _logger.LogWarning("No broker found in database with Id {Id} to delete", id);
            throw new KeyNotFoundException($"No broker found in database with Id {id} to delete");
        }

        await _unitOfWork.BrokerRepository.DeleteAsync(broker);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Deleted broker {@Broker} from database", broker);
    }
}