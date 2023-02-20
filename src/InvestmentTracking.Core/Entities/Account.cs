namespace InvestmentTracking.Core.Entities;

public class Account : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public Guid BrokerId { get; set; }
    public Broker Broker { get; set; }
}