using AutoMapper;
using InvestmentTracking.Core.Dtos;
using InvestmentTracking.Core.Entities;
using InvestmentTracking.Core;

namespace InvestmentTracking.Services.Tests
{
    public class MappingTests
    {
        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            // Act/Assert
            configuration.AssertConfigurationIsValid();
        }

        [Fact]
        public void Mapping_From_BrokerDto_To_Broker_IsValid()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = configuration.CreateMapper();
            var brokerDto = new BrokerDto { /* initialize with test data */ };

            // Act
            var broker = mapper.Map<Broker>(brokerDto);

            // Assert
            // Write assertions to check that the mapping was done correctly
        }

        [Fact]
        public void Mapping_From_Broker_To_BrokerDto_IsValid()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = configuration.CreateMapper();
            var broker = new Broker { /* initialize with test data */ };

            // Act
            var brokerDto = mapper.Map<BrokerDto>(broker);

            // Assert
            // Write assertions to check that the mapping was done correctly
        }
    }
}
