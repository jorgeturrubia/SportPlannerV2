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
        var passwordHash = "hashedpassword";
        var role = UserRole.Admin;

        // Act
        var user = new User(firstName, lastName, email, passwordHash, role);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty();
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
        user.Role.Should().Be(role);
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.UpdatedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_EmptyFirstName_ShouldThrowArgumentException(string firstName)
    {
        // Arrange
        var lastName = "Doe";
        var email = Email.Create("john.doe@example.com");
        var passwordHash = "hashedpassword";
        var role = UserRole.Admin;

        // Act
        var action = () => new User(firstName, lastName, email, passwordHash, role);

        // Assert
        action.Should().Throw<ArgumentException>()
              .WithMessage("*firstName*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_EmptyLastName_ShouldThrowArgumentException(string lastName)
    {
        // Arrange
        var firstName = "John";
        var email = Email.Create("john.doe@example.com");
        var passwordHash = "hashedpassword";
        var role = UserRole.Admin;

        // Act
        var action = () => new User(firstName, lastName, email, passwordHash, role);

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
        var passwordHash = "hashedpassword";
        var role = UserRole.Admin;

        // Act
        var action = () => new User(firstName, lastName, email, passwordHash, role);

        // Assert
        action.Should().Throw<ArgumentException>()
              .WithMessage("*email*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_EmptyPasswordHash_ShouldThrowArgumentException(string passwordHash)
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = Email.Create("john.doe@example.com");
        var role = UserRole.Admin;

        // Act
        var action = () => new User(firstName, lastName, email, passwordHash, role);

        // Assert
        action.Should().Throw<ArgumentException>()
              .WithMessage("*passwordHash*");
    }

    [Fact]
    public void UpdateProfile_ValidParameters_ShouldUpdateUser()
    {
        // Arrange
        var user = new User("John", "Doe", Email.Create("john.doe@example.com"), "hash", UserRole.Admin);
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
    public void ChangePassword_ValidPassword_ShouldUpdatePasswordHash()
    {
        // Arrange
        var user = new User("John", "Doe", Email.Create("john.doe@example.com"), "oldhash", UserRole.Admin);
        var newPasswordHash = "newhash";

        // Act
        user.ChangePassword(newPasswordHash);

        // Assert
        user.PasswordHash.Should().Be(newPasswordHash);
        user.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ChangePassword_EmptyPassword_ShouldThrowArgumentException(string newPasswordHash)
    {
        // Arrange
        var user = new User("John", "Doe", Email.Create("john.doe@example.com"), "oldhash", UserRole.Admin);

        // Act
        var action = () => user.ChangePassword(newPasswordHash);

        // Assert
        action.Should().Throw<ArgumentException>()
              .WithMessage("*password*");
    }
}
