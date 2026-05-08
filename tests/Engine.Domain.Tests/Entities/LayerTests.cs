using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Domain.Tests.Entities;

public sealed class LayerTests
{
    [Fact]
    public void UpdateProperties_UpdatesState()
    {
        var layer = new Layer(LayerId.New(), "Layer", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
        var transform = new LayerTransform(100d, 200d, 2d, 2d, 30d);

        layer.Rename("Updated");
        layer.SetVisibility(false);
        layer.SetTransform(transform);
        layer.SetOpacity(new Opacity(0.4d));
        layer.SetBlendMode(BlendMode.Multiply);

        Assert.Equal("Updated", layer.Name);
        Assert.False(layer.Visibility);
        Assert.Equal(transform, layer.Transform);
        Assert.Equal(new Opacity(0.4d), layer.Opacity);
        Assert.Equal(BlendMode.Multiply, layer.BlendMode);
        Assert.NotNull(layer.EffectStack);
        Assert.Empty(layer.EffectStack.Effects);
    }

    [Fact]
    public void SetBlendMode_WithInvalidEnum_Throws()
    {
        var layer = new Layer(LayerId.New(), "Layer", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));

        Assert.Throws<ArgumentOutOfRangeException>(() => layer.SetBlendMode((BlendMode)999));
    }

    [Fact]
    public void SetAssetReference_UpdatesReference()
    {
        var layer = new Layer(LayerId.New(), "Layer", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
        var newReference = new AssetReference(AssetId.New());

        layer.SetAssetReference(newReference);

        Assert.Equal(newReference, layer.AssetReference);
    }

}
