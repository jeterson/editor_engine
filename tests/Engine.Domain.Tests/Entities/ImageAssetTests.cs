using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Domain.Tests.Entities;

public sealed class ImageAssetTests
{
    [Fact]
    public void Create_WithValidData_CreatesAsset()
    {
        var id = AssetId.New();

        var asset = new ImageAsset(id, "Photo", "image/png");

        Assert.Equal(id, asset.Id);
        Assert.Equal("Photo", asset.Name);
        Assert.Equal("image/png", asset.MimeType);
    }

    [Fact]
    public void Create_WithDefaultId_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ImageAsset(default, "Photo", "image/png"));
    }
}
