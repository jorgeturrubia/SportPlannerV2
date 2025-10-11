using Microsoft.AspNetCore.Mvc.Testing;
using SportPlanner.API;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Text.Json;
using SportPlanner.Domain.Enum;
using System;
using System.Text;

namespace SportPlanner.API.IntegrationTests.Controllers.Planning;

public class MarketplaceControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MarketplaceControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        // Note: In a real scenario, we would need to handle authentication,
        // either by mocking it or by generating a valid test token.
        // For now, these tests assume endpoints are reachable.
    }

    [Fact]
    public async Task Search_ReturnsSuccessStatusCode()
    {
        // Arrange
    var sport = Sport.Football;
        var url = $"/api/planning/marketplace?sport={sport}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status 200-299
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }

    [Fact]
    public async Task GetById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var url = $"/api/planning/marketplace/{nonExistentId}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Publish_WithInvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var url = "/api/planning/marketplace/publish";
        var invalidRequest = new { Type = "InvalidType", SourceEntityId = Guid.Empty }; // Missing/invalid fields
        var content = new StringContent(JsonSerializer.Serialize(invalidRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}