using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using Xunit;

namespace SportPlanner.Domain.UnitTests.Entities;

public class TeamTests
{
    private readonly Guid _subscriptionId = Guid.NewGuid();
    private readonly Guid _teamCategoryId = Guid.NewGuid();
    private readonly Guid _genderId = Guid.NewGuid();
    private readonly Guid _ageGroupId = Guid.NewGuid();

    [Fact]
    public void CreateTeam_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var name = "Real Madrid";
        var color = TeamColor.Blanco;
        var sport = Sport.Football;

        // Act
        var team = new Team(_subscriptionId, name, color, _teamCategoryId, _genderId, _ageGroupId, sport);

        // Assert
        Assert.Equal(_subscriptionId, team.SubscriptionId);
        Assert.Equal(name, team.Name);
        Assert.Equal(color, team.Color);
        Assert.Equal(_teamCategoryId, team.TeamCategoryId);
        Assert.Equal(_genderId, team.GenderId);
        Assert.Equal(_ageGroupId, team.AgeGroupId);
        Assert.Equal(sport, team.Sport);
        Assert.True(team.IsActive);
        Assert.Equal(0, team.CurrentPlayersCount);
        Assert.Equal(22, team.MaxPlayers); // Football default
        Assert.NotEqual(Guid.Empty, team.Id);
    }

    [Fact]
    public void CreateTeam_WithEmptySubscriptionId_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Team(Guid.Empty, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Football));
    }

    [Fact]
    public void CreateTeam_WithEmptyName_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Team(_subscriptionId, "", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Football));
    }

    [Fact]
    public void CreateTeam_WithEmptyTeamCategoryId_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Team(_subscriptionId, "Test Team", TeamColor.Azul, Guid.Empty, _genderId, _ageGroupId, Sport.Football));
    }

    [Fact]
    public void CreateTeam_WithInvalidEmail_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Football,
                     contactEmail: "invalid-email"));
    }

    [Fact]
    public void CreateTeam_WithValidEmail_ShouldSucceed()
    {
        // Arrange
        var validEmail = "coach@example.com";

        // Act
        var team = new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Football,
                           contactEmail: validEmail);

        // Assert
        Assert.Equal(validEmail, team.ContactEmail);
    }

    [Theory]
    [InlineData(Sport.Football, 22)]
    [InlineData(Sport.Basketball, 15)]
    [InlineData(Sport.Handball, 18)]
    public void CreateTeam_WithDifferentSports_ShouldSetCorrectMaxPlayers(Sport sport, int expectedMaxPlayers)
    {
        // Act
        var team = new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, sport);

        // Assert
        Assert.Equal(expectedMaxPlayers, team.MaxPlayers);
    }

    [Fact]
    public void UpdateBasicInfo_WithValidParameters_ShouldUpdate()
    {
        // Arrange
        var team = new Team(_subscriptionId, "Original Name", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Football);
        var newName = "Updated Name";
        var newColor = TeamColor.Rojo;
        var newDescription = "Updated Description";

        // Act
        team.UpdateBasicInfo(newName, newColor, newDescription);

        // Assert
        Assert.Equal(newName, team.Name);
        Assert.Equal(newColor, team.Color);
        Assert.Equal(newDescription, team.Description);
        Assert.NotNull(team.UpdatedAt);
    }

    [Fact]
    public void UpdateContactInfo_WithValidParameters_ShouldUpdate()
    {
        // Arrange
        var team = new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Football);
        var homeVenue = "Santiago Bernabéu";
        var coachName = "Carlo Ancelotti";
        var contactEmail = "coach@realmadrid.com";
        var contactPhone = "+34 123 456 789";

        // Act
        team.UpdateContactInfo(homeVenue, coachName, contactEmail, contactPhone);

        // Assert
        Assert.Equal(homeVenue, team.HomeVenue);
        Assert.Equal(coachName, team.CoachName);
        Assert.Equal(contactEmail, team.ContactEmail);
        Assert.Equal(contactPhone, team.ContactPhone);
        Assert.NotNull(team.UpdatedAt);
    }

    [Fact]
    public void AddPlayer_WhenNotAtMaxCapacity_ShouldIncreaseCount()
    {
        // Arrange
        var team = new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Football);

        // Act
        team.AddPlayer();

        // Assert
        Assert.Equal(1, team.CurrentPlayersCount);
        Assert.NotNull(team.UpdatedAt);
    }

    [Fact]
    public void AddPlayer_WhenAtMaxCapacity_ShouldThrowException()
    {
        // Arrange
        var team = new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Basketball); // Max 15

        // Add max players
        for (int i = 0; i < 15; i++)
        {
            team.AddPlayer();
        }

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => team.AddPlayer());
    }

    [Fact]
    public void RemovePlayer_WhenHasPlayers_ShouldDecreaseCount()
    {
        // Arrange
        var team = new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Football);
        team.AddPlayer();

        // Act
        team.RemovePlayer();

        // Assert
        Assert.Equal(0, team.CurrentPlayersCount);
        Assert.NotNull(team.UpdatedAt);
    }

    [Fact]
    public void RemovePlayer_WhenNoPlayers_ShouldThrowException()
    {
        // Arrange
        var team = new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Football);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => team.RemovePlayer());
    }

    [Fact]
    public void CanAddMorePlayers_WhenNotAtMax_ShouldReturnTrue()
    {
        // Arrange
        var team = new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Basketball); // Max 15
        team.AddPlayer(); // 1 player

        // Act
        var canAdd = team.CanAddMorePlayers();

        // Assert
        Assert.True(canAdd);
    }

    [Fact]
    public void CanAddMorePlayers_WhenAtMax_ShouldReturnFalse()
    {
        // Arrange
        var team = new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Basketball); // Max 15

        // Add max players
        for (int i = 0; i < 15; i++)
        {
            team.AddPlayer();
        }

        // Act
        var canAdd = team.CanAddMorePlayers();

        // Assert
        Assert.False(canAdd);
    }

    [Fact]
    public void AvailableSpots_ShouldReturnCorrectCount()
    {
        // Arrange
        var team = new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Basketball); // Max 15
        team.AddPlayer();
        team.AddPlayer(); // 2 players

        // Act
        var availableSpots = team.AvailableSpots();

        // Assert
        Assert.Equal(13, availableSpots); // 15 - 2 = 13
    }

    [Fact]
    public void UpdateLastMatchDate_ShouldSetDate()
    {
        // Arrange
        var team = new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Football);
        var matchDate = DateTime.UtcNow.Date;

        // Act
        team.UpdateLastMatchDate(matchDate);

        // Assert
        Assert.Equal(matchDate, team.LastMatchDate);
        Assert.NotNull(team.UpdatedAt);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var team = new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Football);

        // Act
        team.Deactivate();

        // Assert
        Assert.False(team.IsActive);
        Assert.NotNull(team.UpdatedAt);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var team = new Team(_subscriptionId, "Test Team", TeamColor.Azul, _teamCategoryId, _genderId, _ageGroupId, Sport.Football);
        team.Deactivate();

        // Act
        team.Activate();

        // Assert
        Assert.True(team.IsActive);
        Assert.NotNull(team.UpdatedAt);
    }
}