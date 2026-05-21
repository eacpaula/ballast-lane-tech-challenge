using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Application.Users;

public sealed class LoginUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordSecurityService _passwordSecurityService;
    private readonly IAuthenticationPayloadFactory _authenticationPayloadFactory;

    public LoginUserHandler(
        IUserRepository userRepository,
        IPasswordSecurityService passwordSecurityService,
        IAuthenticationPayloadFactory authenticationPayloadFactory)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordSecurityService = passwordSecurityService ?? throw new ArgumentNullException(nameof(passwordSecurityService));
        _authenticationPayloadFactory = authenticationPayloadFactory ?? throw new ArgumentNullException(nameof(authenticationPayloadFactory));
    }

    public async Task<LoginUserResult> HandleAsync(
        LoginUserCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return LoginUserResult.Failure(
                errorCode: "ValidationError",
                errorMessage: "email is required.");
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            return LoginUserResult.Failure(
                errorCode: "ValidationError",
                errorMessage: "password is required.");
        }

        var existingUser = await _userRepository.GetByEmailAsync(command.Email.Trim(), cancellationToken);

        if (existingUser is null)
        {
            return LoginUserResult.Failure(
                errorCode: "InvalidCredentials",
                errorMessage: "Invalid email or password.");
        }

        var isValidPassword = _passwordSecurityService.VerifyPassword(
            command.Password,
            existingUser.PasswordHash);

        if (!isValidPassword)
        {
            return LoginUserResult.Failure(
                errorCode: "InvalidCredentials",
                errorMessage: "Invalid email or password.");
        }

        var authenticationPayload = _authenticationPayloadFactory.Create(existingUser);

        return LoginUserResult.Success(existingUser, authenticationPayload);
    }
}
