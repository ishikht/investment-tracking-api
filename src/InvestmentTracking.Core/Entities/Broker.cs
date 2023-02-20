namespace InvestmentTracking.Core.Entities;

public class Broker : IEntity
{
    public string Name { get; set; }
    public Guid Id { get; set; }
}