namespace InvestmentTracking.Core.Dtos;

public class AccountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid BrokerId { get; set; }
}

public class AccountCreateDto
{
    public string Name { get; set; }
    public Guid BrokerId { get; set; }
}

public class AccountUpdateDto
{
    public string Name { get; set; }
}