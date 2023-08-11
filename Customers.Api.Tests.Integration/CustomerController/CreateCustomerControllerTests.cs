using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Customers.Api.Tests.Integration.CustomerController
{
    [Collection("CustomerApi Collection")]
    public class CreateCustomerControllerTests: IAsyncLifetime
    {
        private readonly WebApplicationFactory<IApiMarker> _factory;
        private readonly HttpClient _client;
        private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
            .RuleFor(x => x.FullName, f => f.Person.FullName)
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.DateOfBirth, f => f.Person.DateOfBirth.Date)
            .RuleFor(x => x.GitHubUsername, "PeterZosiak");

        private readonly List<Guid> _createdUsersIds = new();

        public CreateCustomerControllerTests(WebApplicationFactory<IApiMarker> factory)
        {
            // Arrange
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenCustomerIsCreated()
        {
            // Arrange
            var customer = new CustomerRequest
            {
                FullName = "John Doe",
                Email = "john.com@email.com",
                DateOfBirth = new DateTime(1986, 1, 1),
                GitHubUsername = "johndoe",
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/customers/", customer);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
            customerResponse.Should().BeEquivalentTo(customer);

            _createdUsersIds.Add(customerResponse!.Id);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenCustomerIsCreatedWithFakeDate()
        {
            // Arrange
            var customer = _customerGenerator.Generate();

            // Act
            var response = await _client.PostAsJsonAsync($"/customers/", customer);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
            customerResponse.Should().BeEquivalentTo(customer);

            _createdUsersIds.Add(customerResponse!.Id);
        }

       public async Task DisposeAsync()
        {
            foreach (var id in _createdUsersIds)
            {
                await _client.DeleteAsync($"/customers/{id}");
            }
        }

        public async Task InitializeAsync()
        {
            Task.CompletedTask.Wait();
        }
    }
}
