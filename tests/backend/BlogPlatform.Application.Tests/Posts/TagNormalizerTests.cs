using BlogPlatform.Application.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class TagNormalizerTests
{
    [Fact]
    public void Normalize_NullInput_ReturnsEmptyList()
    {
        var result = TagNormalizer.Normalize(null);

        Assert.Empty(result);
    }

    [Fact]
    public void Normalize_EmptyInput_ReturnsEmptyList()
    {
        var result = TagNormalizer.Normalize(Array.Empty<string>());

        Assert.Empty(result);
    }

    [Fact]
    public void Normalize_AllWhitespaceValues_ReturnsEmptyList()
    {
        var result = TagNormalizer.Normalize(new[] { "", "  ", "\t", " " });

        Assert.Empty(result);
    }

    [Fact]
    public void Normalize_TrimsLeadingAndTrailingWhitespace()
    {
        var result = TagNormalizer.Normalize(new[] { "  rust  ", "\tcloud-native\t" });

        Assert.Equal<IEnumerable<string>>(new[] { "rust", "cloud-native" }, result);
    }

    [Fact]
    public void Normalize_DropsEmptyValuesAfterTrimming()
    {
        var result = TagNormalizer.Normalize(new[] { "rust", "", "  ", "tdd" });

        Assert.Equal<IEnumerable<string>>(new[] { "rust", "tdd" }, result);
    }

    [Fact]
    public void Normalize_RemovesCaseInsensitiveDuplicates()
    {
        var result = TagNormalizer.Normalize(new[] { "rust", "Rust", "RUST" });

        Assert.Single(result);
        Assert.Equal("rust", result[0]);
    }

    [Fact]
    public void Normalize_KeepsFirstOccurrenceCasing()
    {
        var result = TagNormalizer.Normalize(new[] { "Cloud-Native", "cloud-native", "CLOUD-NATIVE" });

        Assert.Single(result);
        Assert.Equal("Cloud-Native", result[0]);
    }

    [Fact]
    public void Normalize_PreservesOrderOfSurvivingTags()
    {
        var result = TagNormalizer.Normalize(new[] { "tdd", "architecture", "clean-code" });

        Assert.Equal(3, result.Count);
        Assert.Equal("tdd", result[0]);
        Assert.Equal("architecture", result[1]);
        Assert.Equal("clean-code", result[2]);
    }

    [Fact]
    public void Normalize_MixedInput_AppliesAllRulesInOrder()
    {
        var result = TagNormalizer.Normalize(new[] { "  rust  ", "cloud-native", "rust", "", "  ", "Rust", "tdd" });

        Assert.Equal(3, result.Count);
        Assert.Equal("rust", result[0]);
        Assert.Equal("cloud-native", result[1]);
        Assert.Equal("tdd", result[2]);
    }

    [Fact]
    public void Normalize_SingleValidTag_ReturnsSingleItem()
    {
        var result = TagNormalizer.Normalize(new[] { "dotnet" });

        Assert.Single(result);
        Assert.Equal("dotnet", result[0]);
    }

    [Fact]
    public void Normalize_DuplicatesWithWhitespace_TrimsThenDeduplicates()
    {
        var result = TagNormalizer.Normalize(new[] { " rust ", "rust", "  rust  " });

        Assert.Single(result);
        Assert.Equal("rust", result[0]);
    }
}
