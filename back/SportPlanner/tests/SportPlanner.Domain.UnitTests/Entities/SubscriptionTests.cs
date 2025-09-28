using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using System;
using Xunit;

namespace SportPlanner.Domain.UnitTests.Entities;

public class SubscriptionTests
{
    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Sport _sport = Sport.Football;

    [Fact]
    public void CreateSubscription_WithValidParameters_ShouldSucceed()
    {
        // Act
        var subscription = new Subscription(_ownerId, SubscriptionType.Team, _sport);

        // Assert
        Assert.Equal(_ownerId, subscription.OwnerId);
        Assert.Equal(SubscriptionType.Team, subscription.Type);
        Assert.Equal(_sport, subscription.Sport);
        Assert.Equal(15, subscription.MaxUsers); // Team limits
        Assert.Equal(15, subscription.MaxTeams);
        Assert.True(subscription.IsActive);
        Assert.True(subscription.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void CreateSubscription_FreeType_ShouldHaveLimit1()
    {
        // Act
        var subscription = new Subscription(_ownerId, SubscriptionType.Free, _sport);

        // Assert
        Assert.Equal(1, subscription.MaxUsers);
        Assert.Equal(1, subscription.MaxTeams);
    }

    [Fact]
    public void CreateSubscription_CoachType_ShouldHaveLimit15()
    {
        // Act
        var subscription = new Subscription(_ownerId, SubscriptionType.Coach, _sport);

        // Assert
        Assert.Equal(15, subscription.MaxUsers);
        Assert.Equal(15, subscription.MaxTeams);
    }

    [Fact]
    public void CreateSubscription_ClubType_ShouldHaveLimit1()
    {
        // Act
        var subscription = new Subscription(_ownerId, SubscriptionType.Club, _sport);

        // Assert
        Assert.Equal(1, subscription.MaxUsers);
        Assert.Equal(1, subscription.MaxTeams);
    }

    [Fact]
    public void UpdateSport_WithValidSport_ShouldUpdate()
    {
        // Arrange
        var subscription = new Subscription(_ownerId, SubscriptionType.Team, Sport.Football);
        var newSport = Sport.Basketball;

        // Act
        subscription.UpdateSport(newSport);

        // Assert
        Assert.Equal(newSport, subscription.Sport);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var subscription = new Subscription(_ownerId, SubscriptionType.Team, _sport);

        // Act
        subscription.Deactivate();

        // Assert
        Assert.False(subscription.IsActive);
    }

    [Fact]
    public void GetCurrentUserCount_WithoutUsers_ShouldReturn1()
    {
        // Arrange
        var subscription = new Subscription(_ownerId, SubscriptionType.Team, _sport);

        // Act
        var count = subscription.GetCurrentUserCount();

        // Assert
        Assert.Equal(1, count); // Owner always counts
    }
}
