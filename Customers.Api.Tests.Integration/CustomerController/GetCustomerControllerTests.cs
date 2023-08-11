using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Customers.Api.Tests.Integration.CustomerController
{
    [Collection("CustomerApi Collection")]
    public class GetCustomerControllerTests
    {
        private readonly WebApplicationFactory<IApiMarker> _factory;
        private readonly HttpClient _client;
       
        public GetCustomerControllerTests(WebApplicationFactory<IApiMarker> factory)
        {
            // Arrange
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Get_Customer_ReturnsNotFound_WhenCustomerDoesNotExists()
        {
            // Act
            var response = await _client.GetAsync($"/customers/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Theory]
        [InlineData("00e1a35e-1cf4-4ab3-a4dd-40ffba91ff7f")]
        public async Task Get_Customer_ReturnsNotFound_WhenCustomerDoesNotExistsParametrized(string guid)
        {
            // Act
            var response = await _client.GetAsync($"/customers/{guid}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private static IEnumerable<object[]> GetCustomers()
        {
            yield return new object[] { "00e1a35e-1cf4-4ab3-a4dd-40ffba91ff7f" };
            yield return new object[] { "00e1a35e-1cf4-4ab3-a4dd-40ffba91ff8f" };
        }

        [Theory]
        [MemberData(nameof(GetCustomers))]
        public async Task Get_Customer_ReturnsNotFound_WhenCustomerDoesNotExistsParametrizedMemberData(string guid)
        {
            // Act
            var response = await _client.GetAsync($"/customers/{guid}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [ClassData(typeof(ClassData))]
        public async Task Get_Customer_ReturnsNotFound_WhenCustomerDoesNotExistsParametrizedClassData(string guid)
        {
            // Act
            var response = await _client.GetAsync($"/customers/{guid}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(Skip = "Not implemented yet")]
        public async Task Get_Customer_ReturnsCustomer_WhenCustomerExists()
        {
            // Act
            var response = await _client.PostAsync($"/customers/", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_Customer_ReturnsNotFound_WhenCustomerDoesNotExists_StatusCode()
        {
            // Act
            var response = await _client.GetAsync($"/customers/{Guid.NewGuid()}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_Customer_ReturnsNotFound_WhenCustomerDoesNotExists_BodyText()
        {
            // Act
            var response = await _client.GetAsync($"/customers/{Guid.NewGuid()}");

            // Assert
            var text = await response.Content.ReadAsStringAsync();
            text.Contains("404");
        }

        [Fact]
        public async Task Get_Customer_ReturnsNotFound_WhenCustomerDoesNotExists_BodyJson()
        {
            // Act
            var response = await _client.GetAsync($"/customers/{Guid.NewGuid()}");

            // Assert
            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            problem!.Title.Should().Be("Not Found");
            problem!.Status.Should().Be(StatusCodes.Status404NotFound);
        }
    }


    public class ClassData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "00e1a35e-1cf4-4ab3-a4dd-40ffba91ff7f" };
            yield return new object[] { "00e1a35e-1cf4-4ab3-a4dd-40ffba91fff" };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
