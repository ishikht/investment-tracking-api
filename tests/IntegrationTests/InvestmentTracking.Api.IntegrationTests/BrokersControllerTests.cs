using InvestmentTracking.Core.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentTracking.Api.IntegrationTests
{
    public class BrokersControllerTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;
        private readonly HttpClient _client;

        public BrokersControllerTests(TestFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async Task CreateBroker_ReturnsCreatedBroker()
        {
            // Arrange
            var broker = new BrokerDto
            {
                Name = "Test Broker",
            };
            var content = new StringContent(JsonConvert.SerializeObject(broker), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/brokers", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsAsync<BrokerDto>();
            Assert.Equal(broker.Name, responseContent.Name);
            Assert.NotEqual(Guid.Empty, responseContent.Id);
        }
    }
}
