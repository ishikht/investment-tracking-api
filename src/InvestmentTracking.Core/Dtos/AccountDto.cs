namespace InvestmentTracking.Core.Dtos;

public class AccountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid BrokerId { get; set; }
}