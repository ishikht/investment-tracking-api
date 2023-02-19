using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;

namespace InvestmentTracking.Data.Repositories;

public class BrokerRepository : Repository<Broker>, IBrokerRepository
{
    public BrokerRepository(SqlDbContext dbContext) : base(dbContext)
    {
    }
}