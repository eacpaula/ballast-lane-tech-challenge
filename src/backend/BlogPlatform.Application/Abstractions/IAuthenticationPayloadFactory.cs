using BlogPlatform.Domain.Users;

namespace BlogPlatform.Application.Abstractions;

public interface IAuthenticationPayloadFactory
{
    string Create(UserAccount user);
}
