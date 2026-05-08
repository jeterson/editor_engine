using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Domain.Tests.Entities;

public sealed class EditorDocumentTests
{
    [Fact]
    public void AddLayer_AddsLayerAndReturnsId()
    {
        var document = new EditorDocument(DocumentId.New(), new CanvasSize(1920, 1080));

        var layerId = document.AddLayer("Background", visibility: true);

        var layer = document.GetLayer(layerId);
        Assert.Equal(layerId, layer.Id);
        Assert.Equal("Background", layer.Name);
        Assert.True(layer.Visibility);
    }

    [Fact]
    public void RemoveLayer_RemovesExistingLayer()
    {
        var document = new EditorDocument(DocumentId.New(), new CanvasSize(800, 600));
        var layerId = document.AddLayer("To Remove");

        var removed = document.RemoveLayer(layerId);

        Assert.True(removed);
        Assert.False(document.EnumerateLayers().Any());
    }

    [Fact]
    public void AddLayer_WithDuplicateId_ThrowsInvalidOperationException()
    {
        var document = new EditorDocument(DocumentId.New(), new CanvasSize(100, 100));
        var id = LayerId.New();

        document.AddLayer(new Layer(id, "A", visibility: true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal));

        Assert.Throws<InvalidOperationException>(() => document.AddLayer(new Layer(id, "B", visibility: false, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal)));
    }

    [Fact]
    public void EnumerateLayers_ReturnsReadOnlyCollectionView()
    {
        var document = new EditorDocument(DocumentId.New(), new CanvasSize(1200, 900));
        document.AddLayer("L1");

        var layers = document.EnumerateLayers();

        Assert.IsAssignableFrom<IReadOnlyList<Layer>>(layers);
        Assert.False(layers is List<Layer>);
    }
}
