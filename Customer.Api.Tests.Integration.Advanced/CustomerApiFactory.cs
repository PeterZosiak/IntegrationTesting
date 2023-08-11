using Customers.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Customer.Api.Tests.Integration.Advanced
{
    public class CustomerApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {
        public const string ValidGithubUserName = "PeterZosiak";
        private readonly GithubApiServer _githubApiServer = new();

        public void Initialize()
        {
            _githubApiServer.Start();
        }

        public async Task InitializeAsync()
        {
            _githubApiServer.Start();
            _githubApiServer.SetupUser(ValidGithubUserName);
        }

        public async Task DisposeAsync()
        {
            _githubApiServer.Dispose();
        }

        override protected void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            });


            builder.ConfigureTestServices(services =>
            {
                services.AddHttpClient("GitHub", httpClient =>
                {
                    httpClient.BaseAddress = new Uri(_githubApiServer.Url);
                    httpClient.DefaultRequestHeaders.Add(
                                               HeaderNames.Accept, "application/vnd.github.v3+json");
                    httpClient.DefaultRequestHeaders.Add(
                                               HeaderNames.UserAgent, $"Course-{Environment.MachineName}");
                });
            });

           
        }

      
    }
}
