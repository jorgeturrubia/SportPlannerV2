using Microsoft.AspNetCore.Mvc.Testing;
using SportPlanner.API;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System;
using System.Text;
using System.Text.Json;
using SportPlanner.Domain.Enum;
using System.Collections.Generic;
using SportPlanner.Application.UseCases.Planning;

namespace SportPlanner.API.IntegrationTests.Controllers.Planning;

public class ItineraryControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ItineraryControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        // Authentication would be handled here for a real test suite.
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        // Arrange
        var url = "/api/planning/itineraries";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }

    [Fact]
    public async Task Create_WithValidModel_ReturnsCreated()
    {
        // Arrange
        // This test assumes an authenticated user with Admin role if authorization were fully implemented.
        var url = "/api/planning/itineraries";
        var request = new
        {
            Name = "Test Itinerary from Integration Test",
            Description = "A plan created via integration testing.",
            Sport = Sport.Football,
            Level = Difficulty.Intermediate,
            Items = new List<object>
            {
                new { MarketplaceItemId = Guid.NewGuid(), Order = 1 }
            }
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithMissingName_ReturnsBadRequest()
    {
        // Arrange
        var url = "/api/planning/itineraries";
        var request = new
        {
            Description = "A plan with no name.",
            Sport = Sport.Tennis,
            Level = Difficulty.Beginner,
            Items = new List<object>()
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}