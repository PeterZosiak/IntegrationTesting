using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Customer.Api.Tests.Integration.Advanced.CustomerController
{
    public class CreateCustomerControllerTests : IClassFixture<CustomerApiFactory>
    {
        private readonly CustomerApiFactory _factory;
        private readonly HttpClient httpClient;
        private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
            .RuleFor(x => x.FullName, f => f.Person.FullName)
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.DateOfBirth, f => f.Person.DateOfBirth.Date)
            .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ValidGithubUserName);

        public CreateCustomerControllerTests(CustomerApiFactory factory)
        {
            _factory = factory;
            httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task Create_CreatesCustomer_WithValidRequest_ReturnsCreated()
        {
            // Arrange
            var customerRequest = _customerGenerator.Generate();

            // Act
            var response = await httpClient.PostAsJsonAsync("/customers", customerRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
            customerResponse.Should().BeEquivalentTo(customerRequest);
            response.Headers.Location.Should().Be($"http://localhost/customers/{customerResponse!.Id}");
        }


        [Fact]
        public async Task Create_ReturnsValidationError_WhenCustomerEmailIsInvalid()
        {
            // Arrange
            const string invalidEmail = "invalid-email";
            var customer = _customerGenerator.Clone()
                .RuleFor(x => x.Email, f => invalidEmail)
                .Generate();

            // Act
            var response = await httpClient.PostAsJsonAsync($"/customers/", customer);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            error!.Status.Should().Be((int)HttpStatusCode.BadRequest);
            error!.Title.Should().Be("One or more validation errors occurred.");
            error!.Errors["Email"][0].Should().Be($"{invalidEmail} is not a valid email address");
        }


        [Fact]
        public async Task Create_ReturnsValidationError_WhenGithubUserNotExists()
        {
            // Arrange
            const string invalidUser = "invalid-user";
            var customer = _customerGenerator.Clone()
                .RuleFor(x => x.GitHubUsername, f => invalidUser)
                .Generate();

            // Act
            var response = await httpClient.PostAsJsonAsync($"/customers/", customer);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            error!.Status.Should().Be((int)HttpStatusCode.BadRequest);
            error!.Title.Should().Be("One or more validation errors occurred.");
            error!.Errors["Customer"][0].Should().Be($"There is no GitHub user with username {invalidUser}");
        }
    }
}
