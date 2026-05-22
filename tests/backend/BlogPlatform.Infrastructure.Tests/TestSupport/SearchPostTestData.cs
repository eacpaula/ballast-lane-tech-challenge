namespace BlogPlatform.Infrastructure.Tests;

public static class SearchPostTestData
{
    public const string SharedSearchTerm = "Blueprint";

    public static async Task<(int OwnedPrivatePostId, int OtherUsersPrivatePostId)> InsertPrivateVisibilityFixturesAsync(
        PostgreSqlIntegrationTestDatabase database,
        CancellationToken cancellationToken = default)
    {
        var userId = await database.GetUserIdByUsernameAsync("user", cancellationToken);
        var adminId = await database.GetUserIdByUsernameAsync("admin", cancellationToken);
        var architectureCategoryId = await database.GetCategoryIdByTitleAsync("Architecture", cancellationToken);

        var ownedPrivatePostId = await database.InsertPostAsync(
            userId,
            architectureCategoryId,
            $"{SharedSearchTerm} for My Draft",
            "Private draft summary for the regular user.",
            "Private regular-user content mentioning the blueprint term.",
            isPublic: false,
            isAvailable: true,
            cancellationToken: cancellationToken);

        var otherUsersPrivatePostId = await database.InsertPostAsync(
            adminId,
            architectureCategoryId,
            $"{SharedSearchTerm} for Admin Draft",
            "Private draft summary for the admin user.",
            "Private admin-only content mentioning the blueprint term.",
            isPublic: false,
            isAvailable: true,
            cancellationToken: cancellationToken);

        return (ownedPrivatePostId, otherUsersPrivatePostId);
    }
}
