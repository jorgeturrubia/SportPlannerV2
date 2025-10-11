using Moq;
using SportPlanner.Application.Interfaces;
using SportPlanner.Application.UseCases.Planning;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;
using SportPlanner.Shared.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SportPlanner.Application.UnitTests.UseCases.Planning;

public class DownloadFromMarketplaceCommandHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IMarketplaceItemRepository> _marketplaceItemRepositoryMock;
    private readonly Mock<ITrainingPlanRepository> _trainingPlanRepositoryMock;
    private readonly Mock<IExerciseRepository> _exerciseRepositoryMock;
    private readonly Mock<IObjectiveRepository> _objectiveRepositoryMock;
    private readonly DownloadFromMarketplaceCommandHandler _handler;

    public DownloadFromMarketplaceCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _marketplaceItemRepositoryMock = new Mock<IMarketplaceItemRepository>();
        _trainingPlanRepositoryMock = new Mock<ITrainingPlanRepository>();
        _exerciseRepositoryMock = new Mock<IExerciseRepository>();
        _objectiveRepositoryMock = new Mock<IObjectiveRepository>();

        _handler = new DownloadFromMarketplaceCommandHandler(
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
        var command = new DownloadFromMarketplaceCommand(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenMarketplaceItemNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _currentUserServiceMock.Setup(s => s.GetSubscriptionId()).Returns(Guid.NewGuid());
        _marketplaceItemRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((MarketplaceItem)null);
        var command = new DownloadFromMarketplaceCommand(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithValidExercise_ClonesAndSavesSuccessfully()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(s => s.GetSubscriptionId()).Returns(subscriptionId);
        _currentUserServiceMock.Setup(s => s.GetUserId()).Returns(userId);

        var sourceExercise = new Exercise(null, ContentOwnership.System, "Test Ex", "Desc", Guid.NewGuid(), Guid.NewGuid(), "system");
        var marketplaceItem = MarketplaceItem.CreateSystemItem(MarketplaceItemType.Exercise, Sport.General, sourceExercise.Id, "Test Item", "Desc", "system");

        _marketplaceItemRepositoryMock.Setup(r => r.GetByIdAsync(marketplaceItem.Id, It.IsAny<CancellationToken>())).ReturnsAsync(marketplaceItem);
        _exerciseRepositoryMock.Setup(r => r.GetByIdAsync(sourceExercise.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sourceExercise);

        var command = new DownloadFromMarketplaceCommand(marketplaceItem.Id);

        // Act
        var newExerciseId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, newExerciseId);
        _exerciseRepositoryMock.Verify(r => r.AddAsync(It.Is<Exercise>(e => e.SubscriptionId == subscriptionId), It.IsAny<CancellationToken>()), Times.Once);
        _marketplaceItemRepositoryMock.Verify(r => r.UpdateAsync(It.Is<MarketplaceItem>(m => m.TotalDownloads == 1), It.IsAny<CancellationToken>()), Times.Once);
    }
}