using FluentAssertions;
using Moq;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;
using SportPlanner.Application.UseCases;
using SportPlanner.Domain.ValueObjects;
using Supabase.Gotrue;
using SBUser = Supabase.Gotrue.User;
using DomainUser = SportPlanner.Domain.Entities.User;
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

        // Create a Supabase session object
        var session = new Session();
        session.User = new SBUser { Email = command.Email };
        session.AccessToken = "supabase-access-token";

        mockUserRepository.Setup(x => x.AddAsync(It.IsAny<DomainUser>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockAuthService.Setup(x => x.SignUpAsync(command.Email, command.Password))
            .ReturnsAsync(session);

        var handler = new RegisterUserCommandHandler(mockUserRepository.Object, mockAuthService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().NotBeEmpty();
        result.FirstName.Should().Be(command.FirstName);
        result.LastName.Should().Be(command.LastName);
        result.Email.Should().Be(command.Email);
        result.Role.Should().Be(SportPlanner.Domain.Entities.UserRole.Admin.ToString());
        result.AccessToken.Should().Be(session.AccessToken);

        mockUserRepository.Verify(x => x.AddAsync(It.IsAny<DomainUser>(), It.IsAny<CancellationToken>()), Times.Once);
        mockAuthService.Verify(x => x.SignUpAsync(command.Email, command.Password), Times.Once);
    }

    [Fact]
    public async Task Handle_SupabaseSignUpFails_ShouldReturnNull()
    {
        // Arrange
        var command = new RegisterUserCommand("John", "Doe", "john.doe@example.com", "Password123");
        var mockUserRepository = new Mock<IUserRepository>();
        var mockAuthService = new Mock<IAuthService>();

        // Sign-up fails (existing user or invalid data)
        mockAuthService.Setup(x => x.SignUpAsync(command.Email, command.Password))
            .ReturnsAsync((Session)null);

        var handler = new RegisterUserCommandHandler(mockUserRepository.Object, mockAuthService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        mockAuthService.Verify(x => x.SignUpAsync(command.Email, command.Password), Times.Once);
        mockUserRepository.Verify(x => x.AddAsync(It.IsAny<DomainUser>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
