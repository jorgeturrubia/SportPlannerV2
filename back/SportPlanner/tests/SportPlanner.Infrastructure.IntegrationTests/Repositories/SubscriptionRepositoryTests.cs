using Microsoft.EntityFrameworkCore;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using SportPlanner.Infrastructure.Data;
using SportPlanner.Infrastructure.Repositories;
using Xunit;

namespace SportPlanner.Infrastructure.IntegrationTests.Repositories;

public class SubscriptionRepositoryTests : IDisposable
{
    private readonly SportPlannerDbContext _context;
    private readonly SubscriptionRepository _repository;

    public SubscriptionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<SportPlannerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new SportPlannerDbContext(options);
        _repository = new SubscriptionRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddSubscriptionToDatabase()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var subscription = new Subscription(ownerId, SubscriptionType.Team, Sport.Football);

        // Act
        await _repository.AddAsync(subscription);

        // Assert
        var added = await _context.Subscriptions.FindAsync(subscription.Id);
        Assert.NotNull(added);
        Assert.Equal(ownerId, added!.OwnerId);
        Assert.Equal(SubscriptionType.Team, added.Type);
    }

    [Fact]
    public async Task GetByOwnerIdAsync_WithExistingSubscription_ShouldReturnSubscription()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var subscription = new Subscription(ownerId, SubscriptionType.Coach, Sport.Basketball);
        await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByOwnerIdAsync(ownerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subscription.Id, result!.Id);
        Assert.Equal(SubscriptionType.Coach, result.Type);
        Assert.Equal(15, result.MaxUsers);
    }

    [Fact]
    public async Task GetByOwnerIdAsync_WithNonExistingOwner_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByOwnerIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingSubscription_ShouldReturnSubscription()
    {
        // Arrange
        var subscription = new Subscription(Guid.NewGuid(), SubscriptionType.Free, Sport.Football);
        await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(subscription.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subscription.Id, result!.Id);
    }

    [Fact]
    public async Task ExistsOwnerIdAsync_WithExistingOwner_ShouldReturnTrue()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var subscription = new Subscription(ownerId, SubscriptionType.Team, Sport.Football);
        await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsOwnerIdAsync(ownerId);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsOwnerIdAsync_WithNonExistingOwner_ShouldReturnFalse()
    {
        // Act
        var exists = await _repository.ExistsOwnerIdAsync(Guid.NewGuid());

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSubscription()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var subscription = new Subscription(ownerId, SubscriptionType.Team, Sport.Football);
        await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        var newSport = Sport.Basketball;
        subscription.UpdateSport(newSport);

        // Act
        await _repository.UpdateAsync(subscription);

        // Assert
        var updated = await _context.Subscriptions.FindAsync(subscription.Id);
        Assert.NotNull(updated);
        Assert.Equal(newSport, updated!.Sport);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
