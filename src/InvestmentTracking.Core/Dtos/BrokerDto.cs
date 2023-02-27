namespace InvestmentTracking.Core.Dtos;
public class BrokerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}


public class BrokerUpdateDto
{
    public string Name { get; set; }
}

public class BrokerCreateDto
{
    public string Name { get; set; }
}