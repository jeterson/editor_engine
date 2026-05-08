using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Domain.Tests.ValueObjects;

public sealed class AssetReferenceTests
{
    [Fact]
    public void Create_WithValidAssetId_CreatesReference()
    {
        var assetId = AssetId.New();

        var reference = new AssetReference(assetId);

        Assert.Equal(assetId, reference.AssetId);
    }

    [Fact]
    public void Create_WithDefaultAssetId_Throws()
    {
        Assert.Throws<ArgumentException>(() => new AssetReference(default));
    }
}
