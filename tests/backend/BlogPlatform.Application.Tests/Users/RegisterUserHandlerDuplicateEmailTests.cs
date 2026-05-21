using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Users;
using BlogPlatform.Domain.Users;

namespace BlogPlatform.Application.Tests.Users;

public class RegisterUserHandlerDuplicateEmailTests
{
    [Fact]
    public async Task HandleAsync_WithDuplicateEmail_ReturnsDuplicateEmailFailure()
    {
        var existingUser = UserAccount.Rehydrate(
            id: 42,
            nameOrUsername: "existing-user",
            email: "alice@example.com",
            passwordHash: "hashed-password");

        var userRepository = new DuplicateUserRepository(existingUser);
        var passwordSecurityService = new TrackingPasswordSecurityService();
        var handler = new RegisterUserHandler(userRepository, passwordSecurityService);

        var command = new RegisterUserCommand(
            NameOrUsername: "alice",
            Email: "alice@example.com",
            Password: "StrongPassword123!");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("DuplicateEmail", result.ErrorCode);
        Assert.Equal("The email address is already registered.", result.ErrorMessage);
        Assert.Empty(passwordSecurityService.HashedPasswords);
        Assert.False(userRepository.CreateWasCalled);
    }

    private sealed class DuplicateUserRepository(UserAccount existingUser) : IUserRepository
    {
        public bool CreateWasCalled { get; private set; }

        public Task<UserAccount> CreateAsync(UserAccount user, CancellationToken cancellationToken = default)
        {
            CreateWasCalled = true;
            return Task.FromResult(user);
        }

        public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<UserAccount?>(existingUser);
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
