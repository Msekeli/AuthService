using System.Threading;
using System.Threading.Tasks;
using AuthService.Application.DTOs;
using AuthService.Application.UseCases;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Moq;
using Xunit;

namespace AuthService.UnitTests;

public class RegisterUserCommandTests
{
    private const string ClientId = "interview-prep";
    private const string OtherClientId = "smartwheel";

    private static (Mock<IUserRepository> repo, Mock<IPasswordHasher> hasher, Mock<IClientRegistry> registry) CreateMocks(
        bool validClient = true)
    {
        var repo = new Mock<IUserRepository>();
        var hasher = new Mock<IPasswordHasher>();
        var registry = new Mock<IClientRegistry>();

        registry.Setup(r => r.IsValidClient(It.IsAny<string>())).Returns(validClient);

        return (repo, hasher, registry);
    }

    [Theory]
    [InlineData("Derz", "Admin", "derz@test.com", "Password123!")]
    [InlineData("John", "Doe", "john@test.com", "SecurePass1!")]
    public async Task Register_Should_Create_User_When_Email_Not_Exists_For_Client(
        string firstName,
        string lastName,
        string email,
        string password)
    {
        // Arrange
        var (repo, hasher, registry) = CreateMocks();

        repo.Setup(r => r.ExistsByEmailAsync(ClientId, email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        repo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        hasher.Setup(h => h.Hash(password)).Returns("hashed-password");

        var command = new RegisterUserCommand(repo.Object, hasher.Object, registry.Object);

        var request = new RegisterUserRequest
        {
            ClientId = ClientId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password
        };

        // Act
        await command.ExecuteAsync(request);

        // Assert
        repo.Verify(
            r => r.AddAsync(
                It.Is<User>(u =>
                    u.ClientId == ClientId &&
                    u.Email == email &&
                    u.FirstName == firstName &&
                    u.LastName == lastName &&
                    u.PasswordHash == "hashed-password"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("existing@test.com")]
    public async Task Register_Should_Throw_When_Email_Already_Exists_For_Same_Client(
        string email)
    {
        // Arrange
        var (repo, hasher, registry) = CreateMocks();

        repo.Setup(r => r.ExistsByEmailAsync(ClientId, email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new RegisterUserCommand(repo.Object, hasher.Object, registry.Object);

        var request = new RegisterUserRequest
        {
            ClientId = ClientId,
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = "Password123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            command.ExecuteAsync(request));
    }

    [Fact]
    public async Task Register_Should_Succeed_When_Same_Email_Used_Under_Different_Client()
    {
        // Arrange: email already exists for ClientId, but request targets OtherClientId
        var (repo, hasher, registry) = CreateMocks();
        const string email = "shared@test.com";

        repo.Setup(r => r.ExistsByEmailAsync(ClientId, email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        repo.Setup(r => r.ExistsByEmailAsync(OtherClientId, email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed-password");

        var command = new RegisterUserCommand(repo.Object, hasher.Object, registry.Object);

        var request = new RegisterUserRequest
        {
            ClientId = OtherClientId,
            FirstName = "Jane",
            LastName = "Doe",
            Email = email,
            Password = "Password123!"
        };

        // Act
        await command.ExecuteAsync(request);

        // Assert
        repo.Verify(
            r => r.AddAsync(It.Is<User>(u => u.ClientId == OtherClientId && u.Email == email), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Register_Should_Throw_When_Client_Is_Unknown()
    {
        // Arrange
        var (repo, hasher, registry) = CreateMocks(validClient: false);

        var command = new RegisterUserCommand(repo.Object, hasher.Object, registry.Object);

        var request = new RegisterUserRequest
        {
            ClientId = "not-a-real-client",
            FirstName = "Test",
            LastName = "User",
            Email = "someone@test.com",
            Password = "Password123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            command.ExecuteAsync(request));

        repo.Verify(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
