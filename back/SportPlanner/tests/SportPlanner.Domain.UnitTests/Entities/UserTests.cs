using FluentAssertions;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.ValueObjects;
using Xunit;

namespace SportPlanner.Domain.UnitTests.Entities;

public class UserTests
{
    [Fact]
    public void Create_ValidParameters_ShouldCreateUser()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = Email.Create("john.doe@example.com");
        var role = UserRole.Admin;
        var supabaseUserId = "95b401f6-be14-43a5-bc1d-6a503cfbdfe3";

        // Act
        var user = new User(firstName, lastName, email, role, supabaseUserId);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty();
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.Email.Should().Be(email);
        user.SupabaseUserId.Should().Be(supabaseUserId);
        user.Role.Should().Be(role);
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.UpdatedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_EmptyFirstName_ShouldThrowArgumentException(string? firstName)
    {
        // Arrange
        var lastName = "Doe";
        var email = Email.Create("john.doe@example.com");
        var role = UserRole.Admin;

        // Act
        var action = () => new User(firstName, lastName, email, role);

        // Assert
        action.Should().Throw<ArgumentException>()
              .WithMessage("*firstName*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_EmptyLastName_ShouldThrowArgumentException(string? lastName)
    {
        // Arrange
        var firstName = "John";
        var email = Email.Create("john.doe@example.com");
        var role = UserRole.Admin;

        // Act
        var action = () => new User(firstName, lastName, email, role);

        // Assert
        action.Should().Throw<ArgumentException>()
              .WithMessage("*lastName*");
    }

    [Fact]
    public void Create_NullEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        Email email = null!;
        var role = UserRole.Admin;

        // Act
        var action = () => new User(firstName, lastName, email, role);

        // Assert
        action.Should().Throw<ArgumentException>()
              .WithMessage("*email*");
    }

    [Fact]
    public void UpdateProfile_ValidParameters_ShouldUpdateUser()
    {
        // Arrange
        var user = new User("John", "Doe", Email.Create("john.doe@example.com"), UserRole.Admin);
        var newFirstName = "Jane";
        var newLastName = "Smith";

        // Act
        user.UpdateProfile(newFirstName, newLastName);

        // Assert
        user.FirstName.Should().Be(newFirstName);
        user.LastName.Should().Be(newLastName);
        user.UpdatedAt.Should().NotBeNull();
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SyncSupabaseUserId_ValidUserId_ShouldUpdateSupabaseId()
    {
        // Arrange
        var user = new User("John", "Doe", Email.Create("john.doe@example.com"), UserRole.Admin);
        var supabaseUserId = "95b401f6-be14-43a5-bc1d-6a503cfbdfe3";

        // Act
        user.SyncSupabaseUserId(supabaseUserId);

        // Assert
        user.SupabaseUserId.Should().Be(supabaseUserId);
        user.UpdatedAt.Should().NotBeNull();
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void SyncSupabaseUserId_EmptyUserId_ShouldThrowArgumentException(string? supabaseUserId)
    {
        // Arrange
        var user = new User("John", "Doe", Email.Create("john.doe@example.com"), UserRole.Admin);

        // Act
        var action = () => user.SyncSupabaseUserId(supabaseUserId);

        // Assert
        action.Should().Throw<ArgumentException>()
              .WithMessage("*supabaseUserId*");
    }

    [Fact]
    public void UpdateRole_ValidRole_ShouldUpdateUserRole()
    {
        // Arrange
        var user = new User("John", "Doe", Email.Create("john.doe@example.com"), UserRole.Admin);

        // Act
        user.UpdateRole(UserRole.Admin);

        // Assert
        user.Role.Should().Be(UserRole.Admin);
        user.UpdatedAt.Should().NotBeNull();
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
