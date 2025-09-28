using SportPlanner.Domain.Entities;
using Xunit;

namespace SportPlanner.Domain.UnitTests.Entities;

public class GenderTests
{
    [Fact]
    public void CreateGender_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var name = "Masculino";
        var code = "M";
        var description = "Equipos masculinos";

        // Act
        var gender = new Gender(name, code, description);

        // Assert
        Assert.Equal(name, gender.Name);
        Assert.Equal(code.ToUpperInvariant(), gender.Code);
        Assert.Equal(description, gender.Description);
        Assert.True(gender.IsActive);
        Assert.NotEqual(Guid.Empty, gender.Id);
    }

    [Fact]
    public void CreateGender_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var emptyName = "";
        var code = "M";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Gender(emptyName, code));
    }

    [Fact]
    public void CreateGender_WithEmptyCode_ShouldThrowException()
    {
        // Arrange
        var name = "Masculino";
        var emptyCode = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Gender(name, emptyCode));
    }

    [Fact]
    public void CreateGender_WithLowercaseCode_ShouldConvertToUppercase()
    {
        // Arrange
        var name = "Masculino";
        var code = "m";

        // Act
        var gender = new Gender(name, code);

        // Assert
        Assert.Equal("M", gender.Code);
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdate()
    {
        // Arrange
        var gender = new Gender("Original", "O");
        var newName = "Updated Name";
        var newDescription = "Updated Description";

        // Act
        gender.UpdateDetails(newName, newDescription);

        // Assert
        Assert.Equal(newName, gender.Name);
        Assert.Equal(newDescription, gender.Description);
        Assert.NotNull(gender.UpdatedAt);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var gender = new Gender("Test", "T");

        // Act
        gender.Deactivate();

        // Assert
        Assert.False(gender.IsActive);
        Assert.NotNull(gender.UpdatedAt);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var gender = new Gender("Test", "T");
        gender.Deactivate();

        // Act
        gender.Activate();

        // Assert
        Assert.True(gender.IsActive);
        Assert.NotNull(gender.UpdatedAt);
    }
}