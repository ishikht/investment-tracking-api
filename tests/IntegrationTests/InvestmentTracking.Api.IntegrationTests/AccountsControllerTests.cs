using System.Net;
using System.Text;
using InvestmentTracking.Core.Dtos;
using Newtonsoft.Json;

namespace InvestmentTracking.Api.IntegrationTests;

[Trait("TestCategory", "Integration")]
[Collection("Tests collection")]
public class AccountsControllerTests 
{
    private readonly HttpClient _client;

    public AccountsControllerTests(TestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task Test_CreateAccount_ReturnsCreatedAccount()
    {
        // Arrange
        var brokerDto = new BrokerCreateDto { Name = "Test Broker" };
        var brokerContent =
            new StringContent(JsonConvert.SerializeObject(brokerDto), Encoding.UTF8, "application/json");
        var createBrokerResponse = await _client.PostAsync("/brokers", brokerContent);
        var createdBroker = await createBrokerResponse.Content.ReadAsAsync<BrokerDto>();

        var accountDto = new AccountCreateDto
        {
            Name = "Test Account",
            BrokerId = createdBroker.Id
        };
        var content = new StringContent(JsonConvert.SerializeObject(accountDto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/accounts", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsAsync<AccountDto>();
        Assert.Equal(accountDto.Name, responseContent.Name);
        Assert.Equal(createdBroker.Id, responseContent.BrokerId);
        Assert.NotEqual(Guid.Empty, responseContent.Id);
    }

    [Fact]
    public async Task Test_CreateAccount_ReturnsBadRequestStatusCode()
    {
        // Arrange
        var account = new AccountCreateDto(); // Create an empty account DTO to trigger validation errors
        var content = new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/accounts", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Test_GetAccounts_ReturnsOkStatusCode()
    {
        // Arrange
        TestUtils.ClearDatabase(); // Clear the database before creating test accounts

        var broker = new BrokerDto
        {
            Name = "Test Broker"
        };
        var brokerContent = new StringContent(JsonConvert.SerializeObject(broker), Encoding.UTF8, "application/json");
        var createBrokerResponse = await _client.PostAsync("/brokers", brokerContent);
        var createdBroker = await createBrokerResponse.Content.ReadAsAsync<BrokerDto>();

        var account1 = new AccountCreateDto
        {
            BrokerId = createdBroker.Id,
            Name = "Test Account 1"
        };
        var account1Content =
            new StringContent(JsonConvert.SerializeObject(account1), Encoding.UTF8, "application/json");
        await _client.PostAsync("/accounts", account1Content);

        var account2 = new AccountCreateDto
        {
            BrokerId = createdBroker.Id,
            Name = "Test Account 2"
        };
        var account2Content =
            new StringContent(JsonConvert.SerializeObject(account2), Encoding.UTF8, "application/json");
        await _client.PostAsync("/accounts", account2Content);

        // Act
        var response = await _client.GetAsync("/accounts");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsAsync<List<AccountDto>>();
        Assert.Equal(2, responseContent.Count);
        Assert.Contains(responseContent, a => a.Name == account1.Name);
        Assert.Contains(responseContent, a => a.Name == account2.Name);
    }

    [Fact]
    public async Task Test_GetAccountById_ReturnsOkStatusCode()
    {
        // Arrange
        var broker = new BrokerDto
        {
            Name = "Test Broker"
        };
        var createBrokerContent =
            new StringContent(JsonConvert.SerializeObject(broker), Encoding.UTF8, "application/json");
        var createBrokerResponse = await _client.PostAsync("/brokers", createBrokerContent);
        var createdBroker = await createBrokerResponse.Content.ReadAsAsync<BrokerDto>();

        var account = new AccountCreateDto
        {
            BrokerId = createdBroker.Id,
            Name = "Test Account"
        };
        var createAccountContent =
            new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json");
        var createAccountResponse = await _client.PostAsync("/accounts", createAccountContent);
        createAccountResponse.EnsureSuccessStatusCode();
        var createdAccount = await createAccountResponse.Content.ReadAsAsync<AccountDto>();

        // Act
        var getResponse = await _client.GetAsync($"/accounts/{createdAccount.Id}");

        // Assert
        getResponse.EnsureSuccessStatusCode();
        var responseContent = await getResponse.Content.ReadAsAsync<AccountDto>();
        Assert.Equal(createdAccount.Id, responseContent.Id);
        Assert.Equal(createdAccount.Name, responseContent.Name);
        Assert.Equal(createdAccount.BrokerId, responseContent.BrokerId);
    }

    [Fact]
    public async Task Test_GetAccountById_ReturnsNotFoundStatusCode()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/accounts/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task Test_UpdateAccount_ReturnsOkStatusCode()
    {
        // Arrange
        var broker = new BrokerCreateDto
        {
            Name = "Test Broker"
        };
        var createBrokerResponse = await _client.PostAsJsonAsync("/brokers", broker);
        createBrokerResponse.EnsureSuccessStatusCode();
        var createdBroker = await createBrokerResponse.Content.ReadAsAsync<BrokerDto>();

        var account = new AccountCreateDto
        {
            Name = "Test Account",
            BrokerId = createdBroker.Id
        };
        var createResponse = await _client.PostAsJsonAsync("/accounts", account);
        createResponse.EnsureSuccessStatusCode();
        var createdAccount = await createResponse.Content.ReadAsAsync<AccountDto>();

        var updatedAccount = new AccountUpdateDto { Name = "Test Account Updated" };
        var content = new StringContent(JsonConvert.SerializeObject(updatedAccount), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/accounts/{createdAccount.Id}", content);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Test_UpdateAccount_ReturnsBadRequestStatusCode()
    {
        // Arrange
        var updatedAccount = new AccountUpdateDto();
        var content = new StringContent(JsonConvert.SerializeObject(updatedAccount), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/accounts/{Guid.NewGuid()}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Test_DeleteAccount_ReturnsNoContentStatusCode()
    {
        // Arrange
        var broker = new BrokerCreateDto { Name = "Test Broker" };
        var createBrokerContent =
            new StringContent(JsonConvert.SerializeObject(broker), Encoding.UTF8, "application/json");
        var createBrokerResponse = await _client.PostAsync("/brokers", createBrokerContent);
        var createdBroker = await createBrokerResponse.Content.ReadAsAsync<BrokerDto>();

        var account = new AccountCreateDto
        {
            BrokerId = createdBroker.Id,
            Name = "Test Account"
        };
        var createAccountContent =
            new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json");
        var createAccountResponse = await _client.PostAsync("/accounts", createAccountContent);
        var createdAccount = await createAccountResponse.Content.ReadAsAsync<AccountDto>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/accounts/{createdAccount.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Test_DeleteAccount_ReturnsNotFoundStatusCode()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/accounts/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Test_GetAccountsByBrokerId_ReturnsOkStatusCode()
    {
        // Arrange
        var broker = new BrokerCreateDto
        {
            Name = "Test Broker"
        };
        var createBrokerContent =
            new StringContent(JsonConvert.SerializeObject(broker), Encoding.UTF8, "application/json");
        var createBrokerResponse = await _client.PostAsync("/brokers", createBrokerContent);
        createBrokerResponse.EnsureSuccessStatusCode();
        var createdBroker = await createBrokerResponse.Content.ReadAsAsync<BrokerDto>();

        var account1 = new AccountCreateDto
        {
            Name = "Test Account 1",
            BrokerId = createdBroker.Id
        };
        var createAccount1Content =
            new StringContent(JsonConvert.SerializeObject(account1), Encoding.UTF8, "application/json");
        var createAccount1Response = await _client.PostAsync("/accounts", createAccount1Content);
        createAccount1Response.EnsureSuccessStatusCode();

        var account2 = new AccountCreateDto
        {
            Name = "Test Account 2",
            BrokerId = createdBroker.Id
        };
        var createAccount2Content =
            new StringContent(JsonConvert.SerializeObject(account2), Encoding.UTF8, "application/json");
        var createAccount2Response = await _client.PostAsync("/accounts", createAccount2Content);
        createAccount2Response.EnsureSuccessStatusCode();

        // Act
        var response = await _client.GetAsync($"/accounts/broker/{createdBroker.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsAsync<IEnumerable<AccountDto>>();
        Assert.Equal(2, responseContent.Count());
        Assert.Contains(responseContent, a => a.Name == account1.Name && a.BrokerId == account1.BrokerId);
        Assert.Contains(responseContent, a => a.Name == account2.Name && a.BrokerId == account2.BrokerId);
    }

    [Fact]
    public async Task Test_GetBrokerBalance_ReturnsOkStatusCode()
    {
        // Arrange
        var broker = new BrokerCreateDto
        {
            Name = "Test Broker"
        };
        var createBrokerContent =
            new StringContent(JsonConvert.SerializeObject(broker), Encoding.UTF8, "application/json");
        var createBrokerResponse = await _client.PostAsync("/brokers", createBrokerContent);
        var createdBroker = await createBrokerResponse.Content.ReadAsAsync<BrokerDto>();

        var account = new AccountCreateDto
        {
            Name = "Test Account",
            BrokerId = createdBroker.Id
        };
        var createAccountContent =
            new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json");
        var createAccountResponse = await _client.PostAsync("/accounts", createAccountContent);
        var createdAccount = await createAccountResponse.Content.ReadAsAsync<AccountDto>();

        // Act
        var getBalanceResponse = await _client.GetAsync($"/accounts/broker/{createdBroker.Id}/balance");

        // Assert
        getBalanceResponse.EnsureSuccessStatusCode();
        var balance = await getBalanceResponse.Content.ReadAsAsync<decimal>();
        Assert.Equal(0, balance);
    }
}