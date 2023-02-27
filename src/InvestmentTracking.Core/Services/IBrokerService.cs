using InvestmentTracking.Core.Dtos;

namespace InvestmentTracking.Core.Services;

public interface IBrokerService
{
    Task<BrokerDto> AddBrokerAsync(BrokerCreateDto brokerDto);
    IAsyncEnumerable<BrokerDto> GetAllBrokersAsync();
    Task<BrokerDto?> GetBrokerByIdAsync(Guid id);
    Task UpdateBrokerAsync(Guid id, BrokerUpdateDto brokerDto);
    Task DeleteBrokerAsync(Guid id);
}