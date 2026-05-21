using BlogPlatform.Application.Abstractions;
using BlogPlatform.Domain.Users;

namespace BlogPlatform.Application.Users;

public sealed class RegisterUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordSecurityService _passwordSecurityService;

    public RegisterUserHandler(
        IUserRepository userRepository,
        IPasswordSecurityService passwordSecurityService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordSecurityService = passwordSecurityService ?? throw new ArgumentNullException(nameof(passwordSecurityService));
    }

    public async Task<RegisterUserResult> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = command.Email?.Trim() ?? string.Empty;

        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (existingUser is not null)
        {
            return RegisterUserResult.Failure(
                errorCode: "DuplicateEmail",
                errorMessage: "The email address is already registered.");
        }

        try
        {
            if (string.IsNullOrWhiteSpace(command.Password))
            {
                throw new ArgumentException("password is required.", nameof(command.Password));
            }

            _ = UserAccount.CreateNew(
                nameOrUsername: command.NameOrUsername,
                email: normalizedEmail,
                passwordHash: "validation-placeholder");

            var passwordHash = _passwordSecurityService.HashPassword(command.Password);

            var newUser = UserAccount.CreateNew(
                nameOrUsername: command.NameOrUsername,
                email: normalizedEmail,
                passwordHash: passwordHash);

            var savedUser = await _userRepository.CreateAsync(newUser, cancellationToken);

            return RegisterUserResult.Success(savedUser);
        }
        catch (ArgumentException exception)
        {
            return RegisterUserResult.Failure(
                errorCode: "ValidationError",
                errorMessage: exception.Message);
        }
    }
}
