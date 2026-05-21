using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Users;
using BlogPlatform.Domain.Users;

namespace BlogPlatform.Application.Tests.Users;

public class RegisterUserHandlerValidationTests
{
    [Theory]
    [InlineData("alice", "", "StrongPassword123!")]
    [InlineData("alice", "invalid-email", "StrongPassword123!")]
    [InlineData("alice", "alice@example.com", "")]
    [InlineData("alice", "alice@example.com", "   ")]
    public async Task HandleAsync_WithInvalidEmailOrPassword_ReturnsValidationFailure(
        string nameOrUsername,
        string email,
        string password)
    {
        var userRepository = new TrackingUserRepository();
        var passwordSecurityService = new TrackingPasswordSecurityService();
        var handler = new RegisterUserHandler(userRepository, passwordSecurityService);

        var command = new RegisterUserCommand(
            NameOrUsername: nameOrUsername,
            Email: email,
            Password: password);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("ValidationError", result.ErrorCode);
        Assert.True(
            result.ErrorMessage!.Contains("required", StringComparison.OrdinalIgnoreCase) ||
            result.ErrorMessage.Contains("invalid", StringComparison.OrdinalIgnoreCase));
        Assert.Empty(passwordSecurityService.HashedPasswords);
        Assert.False(userRepository.CreateWasCalled);
    }

    private sealed class TrackingUserRepository : IUserRepository
    {
        public bool CreateWasCalled { get; private set; }

        public Task<UserAccount> CreateAsync(UserAccount user, CancellationToken cancellationToken = default)
        {
            CreateWasCalled = true;
            return Task.FromResult(user);
        }

        public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<UserAccount?>(null);
        }
    }

    private sealed class TrackingPasswordSecurityService : IPasswordSecurityService
    {
        public List<string> HashedPasswords { get; } = [];

        public string HashPassword(string password)
        {
            HashedPasswords.Add(password);
            return $"hashed::{password}";
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            throw new NotSupportedException();
        }
    }
}
