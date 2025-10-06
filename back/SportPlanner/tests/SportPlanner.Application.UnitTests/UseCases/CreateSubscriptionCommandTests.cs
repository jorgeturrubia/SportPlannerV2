using Moq;
using SportPlanner.Application.Interfaces;
using SportPlanner.Application.UseCases;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using Xunit;

namespace SportPlanner.Application.UnitTests.UseCases;

public class CreateSubscriptionCommandTests
{
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<ISubscriptionUserRepository> _subscriptionUserRepositoryMock;
    private readonly CreateSubscriptionCommandHandler _handler;

    public CreateSubscriptionCommandTests()
    {
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
            _subscriptionUserRepositoryMock = new Mock<ISubscriptionUserRepository>();
            _currentUserServiceMock.Setup(x => x.GetUserEmail()).Returns("owner@example.com");
            _handler = new CreateSubscriptionCommandHandler(
                _subscriptionRepositoryMock.Object,
                _subscriptionUserRepositoryMock.Object,
                _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithNewOwner_ShouldCreateSubscription()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var command = new CreateSubscriptionCommand(SubscriptionType.Team, Sport.Football);

        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(ownerId);
        _subscriptionRepositoryMock
            .Setup(x => x.ExistsOwnerIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _subscriptionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingOwner_ShouldThrowException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var command = new CreateSubscriptionCommand(SubscriptionType.Team, Sport.Football);

        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(ownerId);
        _subscriptionRepositoryMock
            .Setup(x => x.ExistsOwnerIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldSetCorrectLimits()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var command = new CreateSubscriptionCommand(SubscriptionType.Coach, Sport.Basketball);

        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(ownerId);
        _subscriptionRepositoryMock
            .Setup(x => x.ExistsOwnerIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _subscriptionRepositoryMock.Verify(x => x.AddAsync(It.Is<Subscription>(
            s => s.Type == SubscriptionType.Coach &&
                 s.Sport == Sport.Basketball &&
                 s.MaxUsers == 15 &&
                 s.MaxTeams == 15 &&
                 s.OwnerId == ownerId &&
                 s.IsActive == true
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}
