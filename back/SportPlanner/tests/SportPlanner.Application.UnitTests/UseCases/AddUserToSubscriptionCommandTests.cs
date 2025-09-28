using Moq;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;
using SportPlanner.Application.UseCases;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using Xunit;

namespace SportPlanner.Application.UnitTests.UseCases;

public class AddUserToSubscriptionCommandTests
{
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<ISubscriptionUserRepository> _subscriptionUserRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly AddUserToSubscriptionCommandHandler _handler;

    public AddUserToSubscriptionCommandTests()
    {
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _subscriptionUserRepositoryMock = new Mock<ISubscriptionUserRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _handler = new AddUserToSubscriptionCommandHandler(
            _subscriptionRepositoryMock.Object,
            _subscriptionUserRepositoryMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidOwnerAndUnderLimit_ShouldAddUserToSubscription()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userIdToAdd = Guid.NewGuid();

        var subscription = new Subscription(ownerId, SubscriptionType.Team, Sport.Football);
        var request = new AddUserToSubscriptionRequest(userIdToAdd, UserRole.Athlete);

        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(ownerId);
        _subscriptionRepositoryMock
            .Setup(x => x.GetByIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _subscriptionUserRepositoryMock
            .Setup(x => x.GetActiveUserCountBySubscriptionIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5); // Currently 5 users active
        _subscriptionUserRepositoryMock
            .Setup(x => x.ExistsUserInSubscriptionAsync(subscriptionId, userIdToAdd, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new AddUserToSubscriptionCommand(subscriptionId, request);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _subscriptionUserRepositoryMock.Verify(x => x.AddAsync(It.Is<SubscriptionUser>(
            su => su.SubscriptionId == subscriptionId &&
                  su.UserId == userIdToAdd &&
                  su.RoleInSubscription == UserRole.Athlete &&
                  su.GrantedBy == ownerId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonOwner_ShouldThrowUnauthorized()
    {
        // Arrange
        var nonOwnerId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userIdToAdd = Guid.NewGuid();

        var subscription = new Subscription(Guid.NewGuid(), SubscriptionType.Team, Sport.Football); // Different owner
        var request = new AddUserToSubscriptionRequest(userIdToAdd, UserRole.Athlete);

        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(nonOwnerId);
        _subscriptionRepositoryMock
            .Setup(x => x.GetByIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        var command = new AddUserToSubscriptionCommand(subscriptionId, request);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UserAlreadyInSubscription_ShouldThrowException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userIdToAdd = Guid.NewGuid();

        var subscription = new Subscription(ownerId, SubscriptionType.Team, Sport.Football);
        var request = new AddUserToSubscriptionRequest(userIdToAdd, UserRole.Athlete);

        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(ownerId);
        _subscriptionRepositoryMock
            .Setup(x => x.GetByIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _subscriptionUserRepositoryMock
            .Setup(x => x.ExistsUserInSubscriptionAsync(subscriptionId, userIdToAdd, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new AddUserToSubscriptionCommand(subscriptionId, request);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AtUserLimit_ShouldThrowException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userIdToAdd = Guid.NewGuid();

        var subscription = new Subscription(ownerId, SubscriptionType.Team, Sport.Football);
        var request = new AddUserToSubscriptionRequest(userIdToAdd, UserRole.Athlete);

        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(ownerId);
        _subscriptionRepositoryMock
            .Setup(x => x.GetByIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _subscriptionUserRepositoryMock
            .Setup(x => x.GetActiveUserCountBySubscriptionIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(15); // At limit
        _subscriptionUserRepositoryMock
            .Setup(x => x.ExistsUserInSubscriptionAsync(subscriptionId, userIdToAdd, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new AddUserToSubscriptionCommand(subscriptionId, request);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistingSubscription_ShouldThrowException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userIdToAdd = Guid.NewGuid();

        var request = new AddUserToSubscriptionRequest(userIdToAdd, UserRole.Athlete);

        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(ownerId);
        _subscriptionRepositoryMock
            .Setup(x => x.GetByIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subscription?)null);

        var command = new AddUserToSubscriptionCommand(subscriptionId, request);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}
