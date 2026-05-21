using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Users;
using BlogPlatform.Domain.Users;

namespace BlogPlatform.Application.Tests.Users;

public class LoginUserHandlerSuccessTests
{
    [Fact]
    public async Task HandleAsync_WithValidCredentials_ReturnsAuthenticationResult()
    {
        var existingUser = UserAccount.Rehydrate(
            id: 7,
            nameOrUsername: "alice",
            email: "alice@example.com",
            passwordHash: "hashed-password");

        var userRepository = new TrackingUserRepository(existingUser);
        var passwordSecurityService = new TrackingPasswordSecurityService(isValid: true);
        var payloadFactory = new TrackingAuthenticationPayloadFactory();
        var handler = new LoginUserHandler(userRepository, passwordSecurityService, payloadFactory);

        var command = new LoginUserCommand(
            Email: "alice@example.com",
            Password: "StrongPassword123!");

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(7, result.UserId);
        Assert.Equal("alice", result.NameOrUsername);
        Assert.Equal("alice@example.com", result.Email);
        Assert.Equal("token-for-7", result.AuthenticationPayload);
        Assert.Equal(["StrongPassword123!:hashed-password"], passwordSecurityService.VerificationAttempts);
        Assert.Equal([7], payloadFactory.CreatedPayloadUserIds);
    }

    private sealed class TrackingUserRepository(UserAccount existingUser) : IUserRepository
    {
        public Task<UserAccount> CreateAsync(UserAccount user, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<UserAccount?>(existingUser);
        }
    }

    private sealed class TrackingPasswordSecurityService(bool isValid) : IPasswordSecurityService
    {
        public List<string> VerificationAttempts { get; } = [];

        public string HashPassword(string password)
        {
            throw new NotSupportedException();
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            VerificationAttempts.Add($"{password}:{passwordHash}");
            return isValid;
        }
    }

    private sealed class TrackingAuthenticationPayloadFactory : IAuthenticationPayloadFactory
    {
        public List<int> CreatedPayloadUserIds { get; } = [];

        public string Create(UserAccount user)
        {
            CreatedPayloadUserIds.Add(user.Id);
            return $"token-for-{user.Id}";
        }
    }
}
