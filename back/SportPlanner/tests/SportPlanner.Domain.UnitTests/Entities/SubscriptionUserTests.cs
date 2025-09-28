using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using System;
using Xunit;

namespace SportPlanner.Domain.UnitTests.Entities;

public class SubscriptionUserTests
{
    private readonly Guid _subscriptionId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _grantedBy = Guid.NewGuid();

    [Fact]
    public void CreateSubscriptionUser_WithValidParameters_ShouldSucceed()
    {
        // Act
        var subscriptionUser = new SubscriptionUser(
            _subscriptionId,
            _userId,
            UserRole.Athlete,
            _grantedBy);

        // Assert
        Assert.Equal(_subscriptionId, subscriptionUser.SubscriptionId);
        Assert.Equal(_userId, subscriptionUser.UserId);
        Assert.Equal(UserRole.Athlete, subscriptionUser.RoleInSubscription);
        Assert.Equal(_grantedBy, subscriptionUser.GrantedBy);
        Assert.True(subscriptionUser.GrantedAt <= DateTime.UtcNow);
        Assert.True(subscriptionUser.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void CreateSubscriptionUser_WithCoachRole_ShouldSucceed()
    {
        // Act
        var subscriptionUser = new SubscriptionUser(
            _subscriptionId,
            _userId,
            UserRole.Coach,
            _grantedBy);

        // Assert
        Assert.Equal(UserRole.Coach, subscriptionUser.RoleInSubscription);
    }

    [Fact]
    public void CreateSubscriptionUser_EmptySubscriptionId_ShouldThrow()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new SubscriptionUser(Guid.Empty, _userId, UserRole.Athlete, _grantedBy));

        Assert.Contains("SubscriptionId cannot be empty", exception.Message);
    }

    [Fact]
    public void CreateSubscriptionUser_EmptyUserId_ShouldThrow()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new SubscriptionUser(_subscriptionId, Guid.Empty, UserRole.Athlete, _grantedBy));

        Assert.Contains("UserId cannot be empty", exception.Message);
    }

    [Fact]
    public void CreateSubscriptionUser_EmptyGrantedBy_ShouldThrow()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new SubscriptionUser(_subscriptionId, _userId, UserRole.Athlete, Guid.Empty));

        Assert.Contains("GrantedBy cannot be empty", exception.Message);
    }

    [Fact]
    public void UpdateRole_WithValidRole_ShouldUpdate()
    {
        // Arrange
        var subscriptionUser = new SubscriptionUser(
            _subscriptionId,
            _userId,
            UserRole.Athlete,
            _grantedBy);

        var newRole = UserRole.Coach;

        // Act
        subscriptionUser.UpdateRole(newRole);

        // Assert
        Assert.Equal(newRole, subscriptionUser.RoleInSubscription);
    }

    [Fact]
    public void Remove_ShouldSetRemovedAtAndBy()
    {
        // Arrange
        var subscriptionUser = new SubscriptionUser(
            _subscriptionId,
            _userId,
            UserRole.Athlete,
            _grantedBy);

        var removedBy = Guid.NewGuid();

        // Act
        subscriptionUser.Remove(removedBy);

        // Assert
        Assert.Equal(removedBy, subscriptionUser.RemovedBy);
        Assert.True(subscriptionUser.RemovedAt.HasValue);
        Assert.True(subscriptionUser.RemovedAt.Value <= DateTime.UtcNow);
    }

    [Fact]
    public void IsActive_WhenNotRemoved_ShouldBeTrue()
    {
        // Arrange
        var subscriptionUser = new SubscriptionUser(
            _subscriptionId,
            _userId,
            UserRole.Athlete,
            _grantedBy);

        // Act & Assert
        Assert.True(subscriptionUser.IsActive);
    }

    [Fact]
    public void IsActive_WhenRemoved_ShouldBeFalse()
    {
        // Arrange
        var subscriptionUser = new SubscriptionUser(
            _subscriptionId,
            _userId,
            UserRole.Athlete,
            _grantedBy);

        // Act
        subscriptionUser.Remove(_grantedBy);

        // Assert
        Assert.False(subscriptionUser.IsActive);
    }
}
