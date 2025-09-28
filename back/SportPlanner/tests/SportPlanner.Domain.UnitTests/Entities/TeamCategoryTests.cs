using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using Xunit;

namespace SportPlanner.Domain.UnitTests.Entities;

public class TeamCategoryTests
{
    [Fact]
    public void CreateTeamCategory_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var name = "Nivel A";
        var code = "NIVEL_A";
        var sport = Sport.Football;
        var description = "Categoría principal";
        var sortOrder = 1;

        // Act
        var category = new TeamCategory(name, code, sport, description, sortOrder);

        // Assert
        Assert.Equal(name, category.Name);
        Assert.Equal(code.ToUpperInvariant(), category.Code);
        Assert.Equal(sport, category.Sport);
        Assert.Equal(description, category.Description);
        Assert.Equal(sortOrder, category.SortOrder);
        Assert.True(category.IsActive);
        Assert.NotEqual(Guid.Empty, category.Id);
    }

    [Fact]
    public void CreateTeamCategory_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var emptyName = "";
        var code = "TEST";
        var sport = Sport.Football;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new TeamCategory(emptyName, code, sport));
    }

    [Fact]
    public void CreateTeamCategory_WithEmptyCode_ShouldThrowException()
    {
        // Arrange
        var name = "Test Category";
        var emptyCode = "";
        var sport = Sport.Football;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new TeamCategory(name, emptyCode, sport));
    }

    [Fact]
    public void CreateTeamCategory_WithLowercaseCode_ShouldConvertToUppercase()
    {
        // Arrange
        var name = "Test Category";
        var code = "test_code";
        var sport = Sport.Football;

        // Act
        var category = new TeamCategory(name, code, sport);

        // Assert
        Assert.Equal("TEST_CODE", category.Code);
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdate()
    {
        // Arrange
        var category = new TeamCategory("Original", "ORIG", Sport.Football);
        var newName = "Updated Name";
        var newDescription = "Updated Description";
        var newSortOrder = 5;

        // Act
        category.UpdateDetails(newName, newDescription, newSortOrder);

        // Assert
        Assert.Equal(newName, category.Name);
        Assert.Equal(newDescription, category.Description);
        Assert.Equal(newSortOrder, category.SortOrder);
        Assert.NotNull(category.UpdatedAt);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var category = new TeamCategory("Test", "TEST", Sport.Football);

        // Act
        category.Deactivate();

        // Assert
        Assert.False(category.IsActive);
        Assert.NotNull(category.UpdatedAt);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var category = new TeamCategory("Test", "TEST", Sport.Football);
        category.Deactivate();

        // Act
        category.Activate();

        // Assert
        Assert.True(category.IsActive);
        Assert.NotNull(category.UpdatedAt);
    }
}