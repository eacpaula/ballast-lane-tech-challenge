using BlogPlatform.Application.Abstractions;
using BlogPlatform.Infrastructure.Caching;
using BlogPlatform.Infrastructure.Categories;
using BlogPlatform.Infrastructure.Configuration;
using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Infrastructure.Posts;
using BlogPlatform.Infrastructure.Reactions;
using BlogPlatform.Infrastructure.Security;
using BlogPlatform.Infrastructure.Users;
using Microsoft.Extensions.DependencyInjection;

namespace BlogPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBlogPlatformInfrastructure(
        this IServiceCollection services,
        PostgreSqlConnectionSettings databaseSettings,
        RedisCacheSettings redisSettings,
        JwtTokenSettings jwtSettings)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(databaseSettings);
        ArgumentNullException.ThrowIfNull(redisSettings);
        ArgumentNullException.ThrowIfNull(jwtSettings);

        services.AddSingleton(databaseSettings);
        services.AddSingleton(redisSettings);
        services.AddSingleton(jwtSettings);
        services.AddSingleton<NpgsqlConnectionFactory>();
        services.AddSingleton<RedisConnectionFactory>();
        services.AddScoped<IPostListCache, RedisPostListCache>();
        services.AddScoped<IUserRepository, PostgreSqlUserRepository>();
        services.AddScoped<IPostRepository, PostgreSqlPostRepository>();
        services.AddScoped<ICategoryRepository, PostgreSqlCategoryRepository>();
        services.AddScoped<IPostReactionRepository, PostgreSqlPostReactionRepository>();
        services.AddScoped<IPasswordSecurityService, PostgreSqlPasswordSecurityService>();
        services.AddScoped<IAuthenticationPayloadFactory, JwtAuthenticationPayloadFactory>();

        return services;
    }
}
