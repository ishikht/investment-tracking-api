using System.Net;
using System.Text;
using InvestmentTracking.Core.Dtos;
using Newtonsoft.Json;

namespace InvestmentTracking.Api.IntegrationTests;

[Trait("TestCategory", "Integration")]
[Collection("Tests collection")]
public class BrokersControllerTests 
{
    private readonly HttpClient _client;
    private readonly TestFixture _fixture;

    public BrokersControllerTests(TestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
    }

    [Fact]
    public async Task CreateBroker_ReturnsCreatedBroker()
    {
        // Arrange
        var broker = new BrokerCreateDto
        {
            Name = "Test Broker"
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

    [Fact]
    public async Task CreateBroker_ReturnsBadRequestStatusCode()
    {
        // Arrange
        var broker = new BrokerCreateDto(); // Create an empty broker DTO to trigger validation errors
        var content = new StringContent(JsonConvert.SerializeObject(broker), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/brokers", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetBrokers_ReturnsOkStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/brokers");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Test_GetBrokerById_ReturnsOkStatusCode()
    {
        // Arrange
        var broker = new BrokerDto
        {
            Name = "Test Broker"
        };
        var content = new StringContent(JsonConvert.SerializeObject(broker), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/brokers", content);
        createResponse.EnsureSuccessStatusCode();
        var createdBroker = await createResponse.Content.ReadAsAsync<BrokerDto>();

        // Act
        var getResponse = await _client.GetAsync($"/brokers/{createdBroker.Id}");

        // Assert
        getResponse.EnsureSuccessStatusCode();
        var responseContent = await getResponse.Content.ReadAsAsync<BrokerDto>();
        Assert.Equal(createdBroker.Id, responseContent.Id);
        Assert.Equal(createdBroker.Name, responseContent.Name);
    }

    [Fact]
    public async Task Test_GetBrokerById_ReturnsNotFoundStatusCode()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/brokers/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBroker_ReturnsOkStatusCode()
    {
        // Arrange
        var broker = new BrokerDto
        {
            Name = "Test Broker"
        };
        var createContent = new StringContent(JsonConvert.SerializeObject(broker), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/brokers", createContent);
        var createdBroker = await createResponse.Content.ReadAsAsync<BrokerDto>();

        var updatedBroker = new BrokerUpdateDto { Name = "Test Broker Updated" };
        var content = new StringContent(JsonConvert.SerializeObject(updatedBroker), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/brokers/{createdBroker.Id}", content);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UpdateBroker_ReturnsBadRequestStatusCode()
    {
        // Arrange
        var updatedBroker = new BrokerUpdateDto
        {
            Name = "Test Broker Updated"
        };
        var content = new StringContent(JsonConvert.SerializeObject(updatedBroker), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/brokers/{Guid.NewGuid()}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBroker_ReturnsNoContentStatusCode()
    {
        // Arrange
        var broker = new BrokerDto
        {
            Name = "Test Broker"
        };
        var content = new StringContent(JsonConvert.SerializeObject(broker), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/brokers", content);
        var createdBroker = await createResponse.Content.ReadAsAsync<BrokerDto>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/brokers/{createdBroker.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }


    [Fact]
    public async Task DeleteBroker_ReturnsNotFoundStatusCode()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/brokers/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

}