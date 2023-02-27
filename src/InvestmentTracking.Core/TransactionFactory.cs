using AutoMapper;
using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Entities;

namespace InvestmentTracking.Core;

public class TransactionFactory : ITransactionFactory
{
    private readonly IMapper _mapper;

    public TransactionFactory(IMapper mapper)
    {
        _mapper = mapper;
    }

    public Transaction CreateTransaction(TransactionDto transactionDto)
    {
        if (transactionDto is StockTransactionDto)
            return _mapper.Map<StockTransaction>(transactionDto);
        if (transactionDto is AccountTransactionDto)
            return _mapper.Map<AccountTransaction>(transactionDto);
        if (transactionDto is IncomeTransactionDto)
            return _mapper.Map<IncomeTransaction>(transactionDto);
        throw new ArgumentException($"Invalid transaction DTO type: {transactionDto.GetType().Name}",
            nameof(transactionDto));
    }

    public TransactionDto CreateTransactionDto(Transaction transaction)
    {
        if (transaction is StockTransaction)
            return _mapper.Map<StockTransactionDto>(transaction);
        if (transaction is AccountTransaction)
            return _mapper.Map<AccountTransactionDto>(transaction);
        if (transaction is IncomeTransaction)
            return _mapper.Map<IncomeTransactionDto>(transaction);
        throw new ArgumentException($"Invalid transaction type: {transaction.GetType().Name}", nameof(transaction));
    }
}

public interface ITransactionFactory
{
    Transaction CreateTransaction(TransactionDto transactionDto);
    TransactionDto CreateTransactionDto(Transaction transaction);
}