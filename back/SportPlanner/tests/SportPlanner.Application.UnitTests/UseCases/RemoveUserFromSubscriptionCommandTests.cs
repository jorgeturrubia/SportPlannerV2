using Moq;
using SportPlanner.Application.Interfaces;
using SportPlanner.Application.UseCases;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using Xunit;

namespace SportPlanner.Application.UnitTests.UseCases;

public class RemoveUserFromSubscriptionCommandTests
{
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<ISubscriptionUserRepository> _subscriptionUserRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly RemoveUserFromSubscriptionCommandHandler _handler;

    public RemoveUserFromSubscriptionCommandTests()
    {
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _subscriptionUserRepositoryMock = new Mock<ISubscriptionUserRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _handler = new RemoveUserFromSubscriptionCommandHandler(
            _subscriptionRepositoryMock.Object,
            _subscriptionUserRepositoryMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidOwnerAndExistingUser_ShouldRemoveUserFromSubscription()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userToRemoveId = Guid.NewGuid();

        var subscription = new Subscription(ownerId, SubscriptionType.Team, Sport.Football);
        var subscriptionUser = new SubscriptionUser(subscriptionId, userToRemoveId, UserRole.Athlete, "user@example.com");

        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(ownerId);
        _currentUserServiceMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");
        _subscriptionRepositoryMock
            .Setup(x => x.GetByIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _subscriptionUserRepositoryMock
            .Setup(x => x.GetBySubscriptionAndUserIdAsync(subscriptionId, userToRemoveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscriptionUser);

        var command = new RemoveUserFromSubscriptionCommand(subscriptionId, userToRemoveId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _subscriptionUserRepositoryMock.Verify(x => x.UpdateAsync(It.Is<SubscriptionUser>(
            su => su.Id == subscriptionUser.Id &&
                  su.RemovedAt.HasValue &&
                  su.RemovedBy == "test@example.com"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonOwner_ShouldThrowUnauthorized()
    {
        // Arrange
        var nonOwnerId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userToRemoveId = Guid.NewGuid();

        var subscription = new Subscription(Guid.NewGuid(), SubscriptionType.Team, Sport.Football); // Different owner

        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(nonOwnerId);
        _subscriptionRepositoryMock
            .Setup(x => x.GetByIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        var command = new RemoveUserFromSubscriptionCommand(subscriptionId, userToRemoveId);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistingSubscription_ShouldThrowException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userToRemoveId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(ownerId);
        _subscriptionRepositoryMock
            .Setup(x => x.GetByIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subscription?)null);

        var command = new RemoveUserFromSubscriptionCommand(subscriptionId, userToRemoveId);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistingUserInSubscription_ShouldThrowException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userToRemoveId = Guid.NewGuid();

        var subscription = new Subscription(ownerId, SubscriptionType.Team, Sport.Football);

        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(ownerId);
        _subscriptionRepositoryMock
            .Setup(x => x.GetByIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _subscriptionUserRepositoryMock
            .Setup(x => x.GetBySubscriptionAndUserIdAsync(subscriptionId, userToRemoveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionUser?)null);

        var command = new RemoveUserFromSubscriptionCommand(subscriptionId, userToRemoveId);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithAlreadyRemovedUser_ShouldThrowException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var userToRemoveId = Guid.NewGuid();

        var subscription = new Subscription(ownerId, SubscriptionType.Team, Sport.Football);
        var subscriptionUser = new SubscriptionUser(subscriptionId, userToRemoveId, UserRole.Athlete, "user@example.com");
        subscriptionUser.Remove("another@example.com"); // Already removed

        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(ownerId);
        _subscriptionRepositoryMock
            .Setup(x => x.GetByIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _subscriptionUserRepositoryMock
            .Setup(x => x.GetBySubscriptionAndUserIdAsync(subscriptionId, userToRemoveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscriptionUser);

        var command = new RemoveUserFromSubscriptionCommand(subscriptionId, userToRemoveId);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}
