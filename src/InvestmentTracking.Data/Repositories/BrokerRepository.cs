using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;
using Microsoft.Extensions.Logging;

namespace InvestmentTracking.Data.Repositories;

public class BrokerRepository : Repository<Broker>, IBrokerRepository
{
    public BrokerRepository(SqlDbContext dbContext, ILogger<BrokerRepository> logger) : base(dbContext, logger)
    {
    }
}