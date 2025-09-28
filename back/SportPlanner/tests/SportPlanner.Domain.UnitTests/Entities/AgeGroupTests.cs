using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using Xunit;

namespace SportPlanner.Domain.UnitTests.Entities;

public class AgeGroupTests
{
    [Fact]
    public void CreateAgeGroup_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var name = "Alevín";
        var code = "ALEVIN";
        var minAge = 8;
        var maxAge = 10;
        var sport = Sport.Football;
        var sortOrder = 1;

        // Act
        var ageGroup = new AgeGroup(name, code, minAge, maxAge, sport, sortOrder);

        // Assert
        Assert.Equal(name, ageGroup.Name);
        Assert.Equal(code.ToUpperInvariant(), ageGroup.Code);
        Assert.Equal(minAge, ageGroup.MinAge);
        Assert.Equal(maxAge, ageGroup.MaxAge);
        Assert.Equal(sport, ageGroup.Sport);
        Assert.Equal(sortOrder, ageGroup.SortOrder);
        Assert.True(ageGroup.IsActive);
        Assert.NotEqual(Guid.Empty, ageGroup.Id);
    }

    [Fact]
    public void CreateAgeGroup_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var emptyName = "";
        var code = "TEST";
        var minAge = 8;
        var maxAge = 10;
        var sport = Sport.Football;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new AgeGroup(emptyName, code, minAge, maxAge, sport));
    }

    [Fact]
    public void CreateAgeGroup_WithEmptyCode_ShouldThrowException()
    {
        // Arrange
        var name = "Test Group";
        var emptyCode = "";
        var minAge = 8;
        var maxAge = 10;
        var sport = Sport.Football;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new AgeGroup(name, emptyCode, minAge, maxAge, sport));
    }

    [Fact]
    public void CreateAgeGroup_WithNegativeMinAge_ShouldThrowException()
    {
        // Arrange
        var name = "Test Group";
        var code = "TEST";
        var minAge = -1;
        var maxAge = 10;
        var sport = Sport.Football;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new AgeGroup(name, code, minAge, maxAge, sport));
    }

    [Fact]
    public void CreateAgeGroup_WithMaxAgeLessThanMinAge_ShouldThrowException()
    {
        // Arrange
        var name = "Test Group";
        var code = "TEST";
        var minAge = 15;
        var maxAge = 10;
        var sport = Sport.Football;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new AgeGroup(name, code, minAge, maxAge, sport));
    }

    [Theory]
    [InlineData(8, true)]
    [InlineData(10, true)]
    [InlineData(9, true)]
    [InlineData(7, false)]
    [InlineData(11, false)]
    public void IsValidForAge_ShouldReturnCorrectResult(int age, bool expected)
    {
        // Arrange
        var ageGroup = new AgeGroup("Alevín", "ALEVIN", 8, 10, Sport.Football);

        // Act
        var result = ageGroup.IsValidForAge(age);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdate()
    {
        // Arrange
        var ageGroup = new AgeGroup("Original", "ORIG", 8, 10, Sport.Football);
        var newName = "Updated Name";
        var newMinAge = 12;
        var newMaxAge = 14;
        var newSortOrder = 5;

        // Act
        ageGroup.UpdateDetails(newName, newMinAge, newMaxAge, newSortOrder);

        // Assert
        Assert.Equal(newName, ageGroup.Name);
        Assert.Equal(newMinAge, ageGroup.MinAge);
        Assert.Equal(newMaxAge, ageGroup.MaxAge);
        Assert.Equal(newSortOrder, ageGroup.SortOrder);
        Assert.NotNull(ageGroup.UpdatedAt);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var ageGroup = new AgeGroup("Test", "TEST", 8, 10, Sport.Football);

        // Act
        ageGroup.Deactivate();

        // Assert
        Assert.False(ageGroup.IsActive);
        Assert.NotNull(ageGroup.UpdatedAt);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var ageGroup = new AgeGroup("Test", "TEST", 8, 10, Sport.Football);
        ageGroup.Deactivate();

        // Act
        ageGroup.Activate();

        // Assert
        Assert.True(ageGroup.IsActive);
        Assert.NotNull(ageGroup.UpdatedAt);
    }
}