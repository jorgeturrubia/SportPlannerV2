using Moq;
using SportPlanner.Application.Interfaces;
using SportPlanner.Application.UseCases.Planning;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SportPlanner.Application.UnitTests.UseCases.Planning;

public class CreateItineraryCommandHandlerTests
{
    private readonly Mock<IItineraryRepository> _itineraryRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CreateItineraryCommandHandler _handler;

    public CreateItineraryCommandHandlerTests()
    {
        _itineraryRepositoryMock = new Mock<IItineraryRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _handler = new CreateItineraryCommandHandler(
            _itineraryRepositoryMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserIsInvalid_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(Guid.Empty);
    var command = new CreateItineraryCommand("Test", "Desc", Sport.Football, Difficulty.Beginner, new List<ItineraryItemToAdd>());

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithValidData_CreatesItinerarySuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(userId);

        var itemsToAdd = new List<ItineraryItemToAdd>
        {
            new(Guid.NewGuid(), 1),
            new(Guid.NewGuid(), 2)
        };

        var command = new CreateItineraryCommand("New Itinerary", "A great pack", Sport.Basketball, Difficulty.Intermediate, itemsToAdd);

        // Act
        var newItineraryId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, newItineraryId);
        _itineraryRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Itinerary>(i => i.Name == command.Name && i.Items.Count == 2), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}