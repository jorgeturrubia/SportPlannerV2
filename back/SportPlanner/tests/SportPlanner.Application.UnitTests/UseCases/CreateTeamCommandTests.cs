using Moq;
using SportPlanner.Application.Interfaces;
using SportPlanner.Application.UseCases;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using Xunit;

namespace SportPlanner.Application.UnitTests.UseCases;

public class CreateTeamCommandTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<ITeamCategoryRepository> _teamCategoryRepositoryMock;
    private readonly Mock<IGenderRepository> _genderRepositoryMock;
    private readonly Mock<IAgeGroupRepository> _ageGroupRepositoryMock;
    private readonly Mock<ISubscriptionUserRepository> _subscriptionUserRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CreateTeamCommandHandler _handler;

    private readonly Guid _subscriptionId = Guid.NewGuid();
    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Guid _teamCategoryId = Guid.NewGuid();
    private readonly Guid _genderId = Guid.NewGuid();
    private readonly Guid _ageGroupId = Guid.NewGuid();

    public CreateTeamCommandTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _teamCategoryRepositoryMock = new Mock<ITeamCategoryRepository>();
        _genderRepositoryMock = new Mock<IGenderRepository>();
        _ageGroupRepositoryMock = new Mock<IAgeGroupRepository>();
        _subscriptionUserRepositoryMock = new Mock<ISubscriptionUserRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _handler = new CreateTeamCommandHandler(
            _teamRepositoryMock.Object,
            _subscriptionRepositoryMock.Object,
            _teamCategoryRepositoryMock.Object,
            _genderRepositoryMock.Object,
            _ageGroupRepositoryMock.Object,
            _subscriptionUserRepositoryMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateTeam()
    {
        // Arrange
        var command = new CreateTeamCommand(
            _subscriptionId,
            "Real Madrid",
            TeamColor.Blanco,
            _teamCategoryId,
            _genderId,
            _ageGroupId,
            "Equipo profesional");

        var subscription = new Subscription(_ownerId, SubscriptionType.Team, Sport.Football);
        var teamCategory = new TeamCategory("Nivel A", "NIVEL_A", Sport.Football);
        var gender = new Gender("Masculino", "M");
        var ageGroup = new AgeGroup("Senior", "SENIOR", 18, 40, Sport.Football);

        SetupValidScenario(subscription, teamCategory, gender, ageGroup);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _teamRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInactiveSubscription_ShouldThrowException()
    {
        // Arrange
        var command = CreateValidCommand();
        var subscription = new Subscription(_ownerId, SubscriptionType.Team, Sport.Football);
        subscription.Deactivate();

        _subscriptionRepositoryMock.Setup(x => x.GetByIdAsync(_subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(_ownerId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Subscription not found or inactive", exception.Message);
    }

    [Fact]
    public async Task Handle_WithUnauthorizedUser_ShouldThrowException()
    {
        // Arrange
        var command = CreateValidCommand();
        var subscription = new Subscription(_ownerId, SubscriptionType.Team, Sport.Football);
        var unauthorizedUserId = Guid.NewGuid();

        _subscriptionRepositoryMock.Setup(x => x.GetByIdAsync(_subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(unauthorizedUserId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("User not authorized to create teams", exception.Message);
    }

    [Fact]
    public async Task Handle_WithMaxTeamsReached_ShouldThrowException()
    {
        // Arrange
        var command = CreateValidCommand();
        var subscription = new Subscription(_ownerId, SubscriptionType.Free, Sport.Football); // Max 1 team

        _subscriptionRepositoryMock.Setup(x => x.GetByIdAsync(_subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(_ownerId);
        _teamRepositoryMock.Setup(x => x.CountActiveTeamsBySubscriptionAsync(_subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1); // Already at max

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Maximum team limit", exception.Message);
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ShouldThrowException()
    {
        // Arrange
        var command = CreateValidCommand();
        var subscription = new Subscription(_ownerId, SubscriptionType.Team, Sport.Football);

        _subscriptionRepositoryMock.Setup(x => x.GetByIdAsync(_subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(_ownerId);
        _teamRepositoryMock.Setup(x => x.CountActiveTeamsBySubscriptionAsync(_subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _teamRepositoryMock.Setup(x => x.ExistsWithNameInSubscriptionAsync(_subscriptionId, command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("already exists in this subscription", exception.Message);
    }

    [Fact]
    public async Task Handle_WithInactiveTeamCategory_ShouldThrowException()
    {
        // Arrange
        var command = CreateValidCommand();
        var subscription = new Subscription(_ownerId, SubscriptionType.Team, Sport.Football);
        var teamCategory = new TeamCategory("Nivel A", "NIVEL_A", Sport.Football);
        teamCategory.Deactivate();

        SetupBasicValidation(subscription);
        _teamCategoryRepositoryMock.Setup(x => x.GetByIdAsync(_teamCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teamCategory);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Team category not found or inactive", exception.Message);
    }

    [Fact]
    public async Task Handle_WithMismatchedSport_ShouldThrowException()
    {
        // Arrange
        var command = CreateValidCommand();
        var subscription = new Subscription(_ownerId, SubscriptionType.Team, Sport.Football);
        var teamCategory = new TeamCategory("Nivel A", "NIVEL_A", Sport.Basketball); // Different sport
        var gender = new Gender("Masculino", "M");
        var ageGroup = new AgeGroup("Senior", "SENIOR", 18, 40, Sport.Football);

        SetupBasicValidation(subscription);
        _teamCategoryRepositoryMock.Setup(x => x.GetByIdAsync(_teamCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teamCategory);
        _genderRepositoryMock.Setup(x => x.GetByIdAsync(_genderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(gender);
        _ageGroupRepositoryMock.Setup(x => x.GetByIdAsync(_ageGroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ageGroup);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Team category sport does not match subscription sport", exception.Message);
    }

    [Fact]
    public async Task Handle_WithMixedGenderButWrongGender_ShouldThrowException()
    {
        // Arrange
        var command = new CreateTeamCommand(
            _subscriptionId,
            "Mixed Team",
            TeamColor.Verde,
            _teamCategoryId,
            _genderId,
            _ageGroupId,
            AllowMixedGender: true);

        var subscription = new Subscription(_ownerId, SubscriptionType.Team, Sport.Football);
        var teamCategory = new TeamCategory("Nivel A", "NIVEL_A", Sport.Football);
        var gender = new Gender("Masculino", "M"); // Not mixed gender
        var ageGroup = new AgeGroup("Senior", "SENIOR", 18, 40, Sport.Football);

        SetupBasicValidation(subscription);
        _teamCategoryRepositoryMock.Setup(x => x.GetByIdAsync(_teamCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teamCategory);
        _genderRepositoryMock.Setup(x => x.GetByIdAsync(_genderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(gender);
        _ageGroupRepositoryMock.Setup(x => x.GetByIdAsync(_ageGroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ageGroup);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Mixed gender is only allowed when gender is set to 'Mixto'", exception.Message);
    }

    private CreateTeamCommand CreateValidCommand()
    {
        return new CreateTeamCommand(
            _subscriptionId,
            "Test Team",
            TeamColor.Azul,
            _teamCategoryId,
            _genderId,
            _ageGroupId,
            "Test Description");
    }

    private void SetupValidScenario(Subscription subscription, TeamCategory teamCategory, Gender gender, AgeGroup ageGroup)
    {
        SetupBasicValidation(subscription);
        _teamCategoryRepositoryMock.Setup(x => x.GetByIdAsync(_teamCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teamCategory);
        _genderRepositoryMock.Setup(x => x.GetByIdAsync(_genderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(gender);
        _ageGroupRepositoryMock.Setup(x => x.GetByIdAsync(_ageGroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ageGroup);
    }

    private void SetupBasicValidation(Subscription subscription)
    {
        _subscriptionRepositoryMock.Setup(x => x.GetByIdAsync(_subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(_ownerId);
        _teamRepositoryMock.Setup(x => x.CountActiveTeamsBySubscriptionAsync(_subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _teamRepositoryMock.Setup(x => x.ExistsWithNameInSubscriptionAsync(_subscriptionId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }
}