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
        CreateMap<BrokerUpdateDto, Broker>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());        
        CreateMap<BrokerCreateDto, Broker>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<AccountDto, Account>()
            .ForMember(dest => dest.Balance, opt => opt.Ignore())
            .ForMember(dest => dest.Broker, opt => opt.Ignore());
        CreateMap<AccountCreateDto, Account>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Balance, opt => opt.Ignore())
            .ForMember(dest => dest.Broker, opt => opt.Ignore());
        CreateMap<AccountUpdateDto, Account>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Balance, opt => opt.Ignore())
            .ForMember(dest => dest.BrokerId, opt => opt.Ignore())
            .ForMember(dest => dest.Broker, opt => opt.Ignore());
        CreateMap<TransactionDto, Transaction>()
            .ForMember(dest => dest.Account, opt => opt.Ignore());
        CreateMap<StockTransactionDto, StockTransaction>()
            .ForMember(dest => dest.Account, opt => opt.Ignore());
        CreateMap<AccountTransactionDto, AccountTransaction>()
            .ForMember(dest => dest.Account, opt => opt.Ignore());
        CreateMap<IncomeTransactionDto, IncomeTransaction>()
            .ForMember(dest => dest.Account, opt => opt.Ignore());

        // Map from entities to DTOs
        CreateMap<Broker, BrokerDto>();
        CreateMap<Account, AccountDto>();
        CreateMap<Transaction, TransactionDto>();
        CreateMap<StockTransaction, StockTransactionDto>();
        CreateMap<AccountTransaction, AccountTransactionDto>();
        CreateMap<IncomeTransaction, IncomeTransactionDto>();
    }
}