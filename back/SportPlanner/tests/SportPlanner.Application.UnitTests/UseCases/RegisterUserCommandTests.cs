using FluentAssertions;
using Moq;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;
using SportPlanner.Application.UseCases;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.ValueObjects;
using Xunit;

namespace SportPlanner.Application.UnitTests.UseCases;

public class RegisterUserCommandTests
{
    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateUserAndReturnAuthResponse()
    {
        // Arrange
        var command = new RegisterUserCommand("John", "Doe", "john.doe@example.com", "Password123");
        var mockUserRepository = new Mock<IUserRepository>();
        var mockAuthService = new Mock<IAuthService>();

        mockUserRepository.Setup(x => x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var expectedUser = new User("John", "Doe", Email.Create("john.doe@example.com"), "hashedpassword", UserRole.Admin);
        var expectedToken = "jwt-token";
        var expectedResponse = new AuthResponse(
            expectedUser.Id,
            expectedUser.FirstName,
            expectedUser.LastName,
            expectedUser.Email.Value,
            expectedUser.Role.ToString(),
            expectedToken
        );

        mockAuthService.Setup(x => x.HashPassword(command.Password)).Returns("hashedpassword");
        mockAuthService.Setup(x => x.GenerateJwtToken(It.IsAny<User>())).Returns(expectedToken);

        var handler = new RegisterUserCommandHandler(mockUserRepository.Object, mockAuthService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().NotBeEmpty();
        result.FirstName.Should().Be(command.FirstName);
        result.LastName.Should().Be(command.LastName);
        result.Email.Should().Be(command.Email);
        result.Role.Should().Be(UserRole.Admin.ToString());
        result.Token.Should().Be(expectedToken);

        mockUserRepository.Verify(x => x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        mockAuthService.Verify(x => x.HashPassword(command.Password), Times.Once);
        mockAuthService.Verify(x => x.GenerateJwtToken(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmailAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new RegisterUserCommand("John", "Doe", "existing@example.com", "Password123");
        var mockUserRepository = new Mock<IUserRepository>();
        var mockAuthService = new Mock<IAuthService>();

        mockUserRepository.Setup(x => x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new RegisterUserCommandHandler(mockUserRepository.Object, mockAuthService.Object);

        // Act
        var action = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User with this email already exists");

        mockUserRepository.Verify(x => x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockAuthService.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        mockAuthService.Verify(x => x.GenerateJwtToken(It.IsAny<User>()), Times.Never);
    }
}
