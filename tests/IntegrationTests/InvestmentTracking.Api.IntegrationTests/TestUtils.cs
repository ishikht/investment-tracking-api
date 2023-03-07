using Microsoft.Data.SqlClient;

namespace InvestmentTracking.Api.IntegrationTests;

public static class TestUtils
{
    private const string ConnectionString = "Server=.;Database=InvestmentTrackingDb;User Id=sa;Password=M1llions2023;Encrypt=False";


    public static void ClearDatabase()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        using var command = new SqlCommand("DELETE FROM Transactions; DELETE FROM Accounts; DELETE FROM Brokers", connection);
        command.ExecuteNonQuery();
    }
}