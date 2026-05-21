namespace BlogPlatform.Api.Tests.TestSupport;

public static class ApiAuthenticationTestHelper
{
    public static Task<HttpClient> CreateAdminClientAsync(BlogPlatformApiFactory factory)
        => factory.CreateAuthenticatedClientAsync("admin@blogplatform.local", "Admin123!");

    public static Task<HttpClient> CreateUserClientAsync(BlogPlatformApiFactory factory)
        => factory.CreateAuthenticatedClientAsync("user@blogplatform.local", "User123!");
}
