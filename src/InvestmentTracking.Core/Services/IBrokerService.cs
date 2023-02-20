using InvestmentTracking.Core.Entities;

namespace InvestmentTracking.Core.Services;

public interface IBrokerService
{
    Task<Broker> AddBrokerAsync(Broker broker);
    Task<IEnumerable<Broker>> GetAllBrokersAsync();
    Task<Broker> GetBrokerByIdAsync(Guid id);
    Task<Broker> UpdateBrokerAsync(Broker broker);
    Task DeleteBrokerAsync(Guid id);
}