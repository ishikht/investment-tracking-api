namespace InvestmentTracking.Core.Dtos;

public class TransactionDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public decimal Commission { get; set; }
    public DateTime Date { get; set; }
    public Guid AccountId { get; set; }
}

public class StockTransactionDto : TransactionDto
{
    public string Ticker { get; set; }
    public int Shares { get; set; }
}

public class AccountTransactionDto : TransactionDto
{
}

public class IncomeTransactionDto : TransactionDto
{
}
