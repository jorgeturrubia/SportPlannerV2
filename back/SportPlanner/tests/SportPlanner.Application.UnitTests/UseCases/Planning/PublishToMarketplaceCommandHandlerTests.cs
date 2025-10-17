using Moq;
using SportPlanner.Application.Interfaces;
using SportPlanner.Application.UseCases.Planning;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;
using SportPlanner.Domain.ValueObjects;
using SportPlanner.Shared.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SportPlanner.Application.UnitTests.UseCases.Planning;

public class PublishToMarketplaceCommandHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IMarketplaceItemRepository> _marketplaceItemRepositoryMock;
    private readonly Mock<ITrainingPlanRepository> _trainingPlanRepositoryMock;
    private readonly Mock<IExerciseRepository> _exerciseRepositoryMock;
    private readonly Mock<IObjectiveRepository> _objectiveRepositoryMock;
    private readonly PublishToMarketplaceCommandHandler _handler;

    public PublishToMarketplaceCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _marketplaceItemRepositoryMock = new Mock<IMarketplaceItemRepository>();
        _trainingPlanRepositoryMock = new Mock<ITrainingPlanRepository>();
        _exerciseRepositoryMock = new Mock<IExerciseRepository>();
        _objectiveRepositoryMock = new Mock<IObjectiveRepository>();

        _handler = new PublishToMarketplaceCommandHandler(
            _currentUserServiceMock.Object,
            _marketplaceItemRepositoryMock.Object,
            _trainingPlanRepositoryMock.Object,
            _exerciseRepositoryMock.Object,
            _objectiveRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ThrowsUnauthorizedException()
    {
        // Arrange
        _currentUserServiceMock.Setup(s => s.GetSubscriptionId()).Returns(Guid.Empty);
        var command = new PublishToMarketplaceCommand(MarketplaceItemType.TrainingPlan, Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenSourceEntityNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        _currentUserServiceMock.Setup(s => s.GetSubscriptionId()).Returns(subscriptionId);
        _trainingPlanRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((TrainingPlan)null);
        var command = new PublishToMarketplaceCommand(MarketplaceItemType.TrainingPlan, Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenUserTriesToPublishAnotherUsersResource_ThrowsForbiddenException()
    {
        // Arrange
        var userSubscriptionId = Guid.NewGuid();
        var ownerSubscriptionId = Guid.NewGuid();
        _currentUserServiceMock.Setup(s => s.GetSubscriptionId()).Returns(userSubscriptionId);

    var startDate = DateTime.UtcNow.Date;
    var endDate = startDate.AddDays(14);
    var totalWeeks = (int)Math.Ceiling((endDate - startDate).TotalDays / 7);
    var trainingPlan = new TrainingPlan(ownerSubscriptionId, "Other User's Plan", startDate, endDate, new TrainingSchedule(new[] { DayOfWeek.Monday }, new System.Collections.Generic.Dictionary<DayOfWeek,int> { { DayOfWeek.Monday, 1 } }, totalWeeks));
        _trainingPlanRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(trainingPlan);

        var command = new PublishToMarketplaceCommand(MarketplaceItemType.TrainingPlan, trainingPlan.Id);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithValidTrainingPlan_CreatesMarketplaceItemSuccessfully()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(s => s.GetSubscriptionId()).Returns(subscriptionId);
        _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(userId);

    var startDate2 = DateTime.UtcNow.Date;
    var endDate2 = startDate2.AddDays(14);
    var totalWeeks2 = (int)Math.Ceiling((endDate2 - startDate2).TotalDays / 7);
    var trainingPlan = new TrainingPlan(subscriptionId, "My Plan", startDate2, endDate2, new TrainingSchedule(new[] { DayOfWeek.Monday }, new System.Collections.Generic.Dictionary<DayOfWeek,int> { { DayOfWeek.Monday, 1 } }, totalWeeks2));
        _trainingPlanRepositoryMock.Setup(r => r.GetByIdAsync(trainingPlan.Id, It.IsAny<CancellationToken>())).ReturnsAsync(trainingPlan);

        var command = new PublishToMarketplaceCommand(MarketplaceItemType.TrainingPlan, trainingPlan.Id);

        // Act
        var newItemId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, newItemId);
        _marketplaceItemRepositoryMock.Verify(
            r => r.AddAsync(It.Is<MarketplaceItem>(m => m.SourceEntityId == trainingPlan.Id && m.PublishedBySubscriptionId == subscriptionId), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}