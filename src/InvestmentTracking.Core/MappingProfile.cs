using AutoMapper;
using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Entities;

namespace InvestmentTracking.Core;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map from DTOs to entities
        CreateMap<BrokerDto, Broker>();
        CreateMap<AccountDto, Account>();
        CreateMap<TransactionDto, Transaction>();
        CreateMap<StockTransactionDto, StockTransaction>();
        CreateMap<AccountTransactionDto, AccountTransaction>();
        CreateMap<IncomeTransactionDto, IncomeTransaction>();

        // Map from entities to DTOs
        CreateMap<Broker, BrokerDto>();
        CreateMap<Account, AccountDto>();
        CreateMap<Transaction, TransactionDto>();
        CreateMap<StockTransaction, StockTransactionDto>();
        CreateMap<AccountTransaction, AccountTransactionDto>();
        CreateMap<IncomeTransaction, IncomeTransactionDto>();
    }
}