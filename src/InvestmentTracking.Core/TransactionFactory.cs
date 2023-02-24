using AutoMapper;
using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentTracking.Core
{
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
            {
                return _mapper.Map<StockTransaction>(transactionDto);
            }
            else if (transactionDto is AccountTransactionDto)
            {
                return _mapper.Map<AccountTransaction>(transactionDto);
            }
            else if (transactionDto is IncomeTransactionDto)
            {
                return _mapper.Map<IncomeTransaction>(transactionDto);
            }
            else
            {
                throw new ArgumentException($"Invalid transaction DTO type: {transactionDto.GetType().Name}", nameof(transactionDto));
            }
        }

        public TransactionDto CreateTransactionDto(Transaction transaction)
        {
            if (transaction is StockTransaction)
            {
                return _mapper.Map<StockTransactionDto>(transaction);
            }
            else if (transaction is AccountTransaction)
            {
                return _mapper.Map<AccountTransactionDto>(transaction);
            }
            else if (transaction is IncomeTransaction)
            {
                return _mapper.Map<IncomeTransactionDto>(transaction);
            }
            else
            {
                throw new ArgumentException($"Invalid transaction type: {transaction.GetType().Name}", nameof(transaction));
            }
        }
    }

    public interface ITransactionFactory
    {
        Transaction CreateTransaction(TransactionDto transactionDto);
        TransactionDto CreateTransactionDto(Transaction transaction);
    }
}
