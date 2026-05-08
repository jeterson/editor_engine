using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Domain.Tests.ValueObjects;

public sealed class StronglyTypedIdsTests
{
    [Fact]
    public void RequiredIds_New_GeneratesNonDefaultValues()
    {
        Assert.NotEqual(default, DocumentId.New());
        Assert.NotEqual(default, LayerId.New());
        Assert.NotEqual(default, AssetId.New());
        Assert.NotEqual(default, EffectId.New());
    }

    [Fact]
    public void OptionalIds_New_GeneratesNonDefaultValues()
    {
        Assert.NotEqual(default, RenderNodeId.New());
        Assert.NotEqual(default, VersionId.New());
    }

    [Fact]
    public void Equality_UsesUnderlyingValueAndTypeSafety()
    {
        var guid = Guid.NewGuid();

        var documentA = new DocumentId(guid);
        var documentB = new DocumentId(guid);
        var layer = new LayerId(guid);

        Assert.Equal(documentA, documentB);
        Assert.True(documentA == documentB);
        Assert.Equal(documentA.GetHashCode(), documentB.GetHashCode());

        Assert.Equal(documentA.Value, layer.Value);
        Assert.IsType<DocumentId>(documentA);
        Assert.IsType<LayerId>(layer);
    }

    [Fact]
    public void ToString_ReturnsGuidString()
    {
        var guid = Guid.NewGuid();

        Assert.Equal(guid.ToString(), new DocumentId(guid).ToString());
        Assert.Equal(guid.ToString(), new LayerId(guid).ToString());
        Assert.Equal(guid.ToString(), new AssetId(guid).ToString());
        Assert.Equal(guid.ToString(), new EffectId(guid).ToString());
        Assert.Equal(guid.ToString(), new RenderNodeId(guid).ToString());
        Assert.Equal(guid.ToString(), new VersionId(guid).ToString());
    }

    [Fact]
    public void TryParse_WithValidInput_ReturnsTrueAndParsedValue()
    {
        var guid = Guid.NewGuid();
        var text = guid.ToString();

        Assert.True(DocumentId.TryParse(text, out var documentId));
        Assert.Equal(new DocumentId(guid), documentId);

        Assert.True(LayerId.TryParse(text, out var layerId));
        Assert.Equal(new LayerId(guid), layerId);

        Assert.True(AssetId.TryParse(text, out var assetId));
        Assert.Equal(new AssetId(guid), assetId);

        Assert.True(EffectId.TryParse(text, out var effectId));
        Assert.Equal(new EffectId(guid), effectId);

        Assert.True(RenderNodeId.TryParse(text, out var renderNodeId));
        Assert.Equal(new RenderNodeId(guid), renderNodeId);

        Assert.True(VersionId.TryParse(text, out var versionId));
        Assert.Equal(new VersionId(guid), versionId);
    }

    [Fact]
    public void TryParse_WithInvalidInput_ReturnsFalseAndDefault()
    {
        const string invalid = "not-a-guid";

        Assert.False(DocumentId.TryParse(invalid, out var documentId));
        Assert.Equal(default, documentId);

        Assert.False(LayerId.TryParse(invalid, out var layerId));
        Assert.Equal(default, layerId);

        Assert.False(AssetId.TryParse(invalid, out var assetId));
        Assert.Equal(default, assetId);

        Assert.False(EffectId.TryParse(invalid, out var effectId));
        Assert.Equal(default, effectId);

        Assert.False(RenderNodeId.TryParse(invalid, out var renderNodeId));
        Assert.Equal(default, renderNodeId);

        Assert.False(VersionId.TryParse(invalid, out var versionId));
        Assert.Equal(default, versionId);
    }
}
