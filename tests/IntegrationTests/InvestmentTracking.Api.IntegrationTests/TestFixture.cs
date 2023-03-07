using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Commands;
using Ductus.FluentDocker.Services;

namespace InvestmentTracking.Api.IntegrationTests
{
    public class TestFixture : IDisposable
    {
        private static bool _isCreated = false;
        private readonly string _dockerComposeFilePath;
        private readonly int _dockerComposeStartupTimeoutSeconds;
        private ICompositeService _dockerComposeService;

        public HttpClient Client { get; }

        public TestFixture()
        {
            // Define the path to the Docker Compose file
            _dockerComposeFilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "docker-compose.yml");

            // Define the Docker Compose startup timeout in seconds
            _dockerComposeStartupTimeoutSeconds = 60;

            // Create an HTTP client to use in the tests
            Client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

            // Start the Docker Compose configuration
            StartDockerCompose();
        }

        public void Dispose()
        {
            // Stop and remove the Docker Compose configuration
            _dockerComposeService.Stop();
            _dockerComposeService.Dispose();
        }

        private void StartDockerCompose()
        {
            // Build and start the Docker Compose configuration
            _dockerComposeService = new Builder()
                .UseContainer()
                .UseCompose()
                .FromFile(_dockerComposeFilePath)
                .ServiceName("investment-tracking-integration_tests")
                .WaitForPort("api", "80", 30000)
                .Build()
                .Start();

            // Wait for the containers to start up
            WaitForDockerComposeStartup();
        }

        private void WaitForDockerComposeStartup()
        {
            // Define the timeout
            var timeout = TimeSpan.FromSeconds(_dockerComposeStartupTimeoutSeconds);

            // Define the delay between checks
            var delay = TimeSpan.FromSeconds(5);

            // Define the start time
            var startTime = DateTime.Now;

            // Check the status of the containers
            while (DateTime.Now - startTime < timeout)
            {
                var apiContainer = _dockerComposeService.Containers.First(c => c.Name == "investment-tracking-api");

                if (apiContainer.State == ServiceRunningState.Running)
                {
                    try
                    {
                        var response =  Client.GetAsync("swagger/index.html").GetAwaiter().GetResult();
                        if (response.IsSuccessStatusCode) return;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                Thread.Sleep(delay);
            }

            throw new Exception("Docker Compose configuration failed to start.");
        }
    }


    [CollectionDefinition("Tests collection")]
    public class TestsCollection : ICollectionFixture<TestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
