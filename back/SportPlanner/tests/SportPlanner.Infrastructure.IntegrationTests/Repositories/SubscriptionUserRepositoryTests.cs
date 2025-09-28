using Microsoft.EntityFrameworkCore;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using SportPlanner.Infrastructure.Data;
using SportPlanner.Infrastructure.Repositories;
using Xunit;

namespace SportPlanner.Infrastructure.IntegrationTests.Repositories;

public class SubscriptionUserRepositoryTests : IDisposable
{
    private readonly SportPlannerDbContext _context;
    private readonly SubscriptionUserRepository _repository;

    public SubscriptionUserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<SportPlannerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new SportPlannerDbContext(options);
        _repository = new SubscriptionUserRepository(_context);
    }

    [Fact]
    public async Task GetActiveUserCountBySubscriptionIdAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var subscriptionUser1 = new SubscriptionUser(subscriptionId, Guid.NewGuid(), UserRole.Athlete, Guid.NewGuid());
        var subscriptionUser2 = new SubscriptionUser(subscriptionId, Guid.NewGuid(), UserRole.Coach, Guid.NewGuid());
        var subscriptionUser3 = new SubscriptionUser(subscriptionId, Guid.NewGuid(), UserRole.Athlete, Guid.NewGuid());
        subscriptionUser3.Remove(Guid.NewGuid()); // Soft delete - should not count

        await _context.SubscriptionUsers.AddRangeAsync(subscriptionUser1, subscriptionUser2, subscriptionUser3);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.GetActiveUserCountBySubscriptionIdAsync(subscriptionId);

        // Assert
        Assert.Equal(2, count); // Only 2 active users
    }

    [Fact]
    public async Task ExistsUserInSubscriptionAsync_WithActiveUser_ShouldReturnTrue()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subscriptionUser = new SubscriptionUser(subscriptionId, userId, UserRole.Athlete, Guid.NewGuid());

        await _context.SubscriptionUsers.AddAsync(subscriptionUser);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsUserInSubscriptionAsync(subscriptionId, userId);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsUserInSubscriptionAsync_WithRemovedUser_ShouldReturnFalse()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subscriptionUser = new SubscriptionUser(subscriptionId, userId, UserRole.Athlete, Guid.NewGuid());
        subscriptionUser.Remove(Guid.NewGuid()); // Soft delete

        await _context.SubscriptionUsers.AddAsync(subscriptionUser);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsUserInSubscriptionAsync(subscriptionId, userId);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task AddAsync_ShouldAddSubscriptionUserToDatabase()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subscriptionUser = new SubscriptionUser(subscriptionId, userId, UserRole.Athlete, Guid.NewGuid());

        // Act
        await _repository.AddAsync(subscriptionUser);

        // Assert
        var added = await _context.SubscriptionUsers.FindAsync(subscriptionUser.Id);
        Assert.NotNull(added);
        Assert.Equal(subscriptionId, added!.SubscriptionId);
        Assert.Equal(userId, added.UserId);
        Assert.Equal(UserRole.Athlete, added.RoleInSubscription);
        Assert.True(added.IsActive);
    }

    [Fact]
    public async Task GetActiveUsersBySubscriptionIdAsync_ShouldReturnOnlyActiveUsers()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var subscriptionUser1 = new SubscriptionUser(subscriptionId, Guid.NewGuid(), UserRole.Athlete, Guid.NewGuid());
        var subscriptionUser2 = new SubscriptionUser(subscriptionId, Guid.NewGuid(), UserRole.Coach, Guid.NewGuid());
        var subscriptionUser3 = new SubscriptionUser(subscriptionId, Guid.NewGuid(), UserRole.Athlete, Guid.NewGuid());
        subscriptionUser3.Remove(Guid.NewGuid()); // Soft delete

        await _context.SubscriptionUsers.AddRangeAsync(subscriptionUser1, subscriptionUser2, subscriptionUser3);
        await _context.SaveChangesAsync();

        // Act
        var activeUsers = await _repository.GetActiveUsersBySubscriptionIdAsync(subscriptionId);

        // Assert
        Assert.Equal(2, activeUsers.Count);
        Assert.DoesNotContain(activeUsers, su => !su.IsActive);
        Assert.Contains(activeUsers, su => su.RoleInSubscription == UserRole.Athlete);
        Assert.Contains(activeUsers, su => su.RoleInSubscription == UserRole.Coach);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSubscriptionUser()
    {
        // Arrange
        var subscriptionUser = new SubscriptionUser(Guid.NewGuid(), Guid.NewGuid(), UserRole.Athlete, Guid.NewGuid());
        await _context.SubscriptionUsers.AddAsync(subscriptionUser);
        await _context.SaveChangesAsync();

        var newRole = UserRole.Coach;
        subscriptionUser.UpdateRole(newRole);

        // Act
        await _repository.UpdateAsync(subscriptionUser);

        // Assert
        var updated = await _context.SubscriptionUsers.FindAsync(subscriptionUser.Id);
        Assert.NotNull(updated);
        Assert.Equal(newRole, updated!.RoleInSubscription);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
