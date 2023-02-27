using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Entities;

namespace InvestmentTracking.Core.Services;

public interface IBrokerService
{
    Task<BrokerDto> AddBrokerAsync(BrokerDto broker);
    IAsyncEnumerable<BrokerDto> GetAllBrokersAsync();
    Task<BrokerDto?> GetBrokerByIdAsync(Guid id);
    Task UpdateBrokerAsync(Guid id, BrokerUpdateDto brokerDto);
    Task DeleteBrokerAsync(Guid id);
}