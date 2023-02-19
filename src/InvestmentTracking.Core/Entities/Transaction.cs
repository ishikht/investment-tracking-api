namespace InvestmentTracking.Core.Entities;

public class Transaction : IEntity
{
    public decimal Amount { get; set; }
    public decimal Commission { get; set; }
    public DateTime Date { get; set; }
    public Guid AccountId { get; set; }

    public virtual Account Account { get; set; }
    public Guid Id { get; set; }
}

public class StockTransaction : Transaction
{
    public string Ticker { get; set; }
    public int Shares { get; set; }
}

public class AccountTransaction : Transaction
{
}

public class IncomeTransaction : Transaction
{
}