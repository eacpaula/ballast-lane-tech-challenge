using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Users;
using BlogPlatform.Domain.Users;

namespace BlogPlatform.Application.Tests.Users;

public class LoginUserHandlerInvalidCredentialsTests
{
    [Fact]
    public async Task HandleAsync_WithUnknownEmail_ReturnsInvalidCredentialsFailure()
    {
        var userRepository = new MissingUserRepository();
        var passwordSecurityService = new TrackingPasswordSecurityService(isValid: false);
        var payloadFactory = new TrackingAuthenticationPayloadFactory();
        var handler = new LoginUserHandler(userRepository, passwordSecurityService, payloadFactory);

        var command = new LoginUserCommand(
            Email: "alice@example.com",
            Password: "StrongPassword123!");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidCredentials", result.ErrorCode);
        Assert.Equal("Invalid email or password.", result.ErrorMessage);
        Assert.Empty(passwordSecurityService.VerificationAttempts);
        Assert.Empty(payloadFactory.CreatedPayloadUserIds);
    }

    [Fact]
    public async Task HandleAsync_WithIncorrectPassword_ReturnsInvalidCredentialsFailure()
    {
        var existingUser = UserAccount.Rehydrate(
            id: 7,
            nameOrUsername: "alice",
            email: "alice@example.com",
            passwordHash: "hashed-password");

        var userRepository = new ExistingUserRepository(existingUser);
        var passwordSecurityService = new TrackingPasswordSecurityService(isValid: false);
        var payloadFactory = new TrackingAuthenticationPayloadFactory();
        var handler = new LoginUserHandler(userRepository, passwordSecurityService, payloadFactory);

        var command = new LoginUserCommand(
            Email: "alice@example.com",
            Password: "WrongPassword123!");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidCredentials", result.ErrorCode);
        Assert.Equal("Invalid email or password.", result.ErrorMessage);
        Assert.Equal(["WrongPassword123!:hashed-password"], passwordSecurityService.VerificationAttempts);
        Assert.Empty(payloadFactory.CreatedPayloadUserIds);
    }

    private sealed class MissingUserRepository : IUserRepository
    {
        public Task<UserAccount> CreateAsync(UserAccount user, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<UserAccount?>(null);
        }
    }

    private sealed class ExistingUserRepository(UserAccount existingUser) : IUserRepository
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
