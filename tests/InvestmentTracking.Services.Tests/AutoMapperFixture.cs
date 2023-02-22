using AutoMapper;
using InvestmentTracking.Core;

namespace InvestmentTracking.Services.Tests;

public class AutoMapperFixture : IDisposable
{
    public AutoMapperFixture()
    {
        var configuration = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });

        Mapper = configuration.CreateMapper();
    }

    public IMapper Mapper { get; }

    public void Dispose()
    {
        // Reset Mapper after each test
        //Mapper.Reset();
    }
}

[CollectionDefinition("AutoMapper")]
public class AutoMapperCollection : ICollectionFixture<AutoMapperFixture>
{
}