using BlogPlatform.Application.Posts;
using BlogPlatform.Application.Users;
using BlogPlatform.Infrastructure;
using BlogPlatform.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogPlatform.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlogPlatformApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<RegisterUserHandler>();
        services.AddTransient<LoginUserHandler>();
        services.AddTransient<ListPublicPostsHandler>();
        services.AddTransient<ListAvailablePostCategoriesHandler>();
        services.AddTransient<ListOwnedPostsHandler>();
        services.AddTransient<GetOwnedPostByIdHandler>();
        services.AddTransient<GetPublicPostByIdHandler>();
        services.AddTransient<ReactToPostHandler>();
        services.AddTransient<CreateBlogPostHandler>();
        services.AddTransient<EditBlogPostHandler>();
        services.AddTransient<RemoveBlogPostHandler>();
        services.AddTransient<ListAllPostCategoriesHandler>();
        services.AddTransient<CreatePostCategoryHandler>();
        services.AddTransient<UpdatePostCategoryHandler>();
        services.AddTransient<DeactivatePostCategoryHandler>();

        return services;
    }

    public static IServiceCollection AddBlogPlatformInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Configuration.JwtAuthenticationSettings jwtSettings)
    {
        var databaseSettings = PostgreSqlConnectionSettings.FromConfiguration(configuration);
        var redisSettings = RedisCacheSettings.FromConfiguration(configuration);
        return Infrastructure.DependencyInjection.AddBlogPlatformInfrastructure(
            services,
            databaseSettings,
            redisSettings,
            jwtSettings.ToInfrastructureSettings());
    }
}
