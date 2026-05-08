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

        var layerId = document.AddLayer("Background", new AssetReference(AssetId.New()), visibility: true);

        var layer = document.GetLayer(layerId);
        Assert.Equal(layerId, layer.Id);
        Assert.Equal("Background", layer.Name);
        Assert.True(layer.Visibility);
    }

    [Fact]
    public void AddAndRemoveNode_MaintainsRootHierarchy()
    {
        var document = new EditorDocument(DocumentId.New(), new CanvasSize(800, 600));
        var groupId = document.AddLayerGroup("Group");

        var removed = document.RemoveNode(groupId);

        Assert.True(removed);
        Assert.Empty(document.EnumerateNodes());
    }

    [Fact]
    public void AddNode_WithDuplicateId_ThrowsInvalidOperationException()
    {
        var document = new EditorDocument(DocumentId.New(), new CanvasSize(100, 100));
        var id = DocumentNodeId.New();

        document.AddNode(new Layer(id, "A", visibility: true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New())));

        Assert.Throws<InvalidOperationException>(() => document.AddNode(new Layer(id, "B", visibility: false, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()))));
    }

    [Fact]
    public void EnumerateNodes_ReturnsReadOnlyCollectionView()
    {
        var document = new EditorDocument(DocumentId.New(), new CanvasSize(1200, 900));
        document.AddLayer("L1", new AssetReference(AssetId.New()));

        var nodes = document.EnumerateNodes();

        Assert.IsAssignableFrom<IReadOnlyList<DocumentNode>>(nodes);
        Assert.False(nodes is List<DocumentNode>);
    }

    [Fact]
    public void RemoveNode_RemovesNodeFromSelectionAndKeepsSelectionConsistent()
    {
        var document = new EditorDocument(DocumentId.New(), new CanvasSize(800, 600));
        var first = document.AddLayer("A", new AssetReference(AssetId.New()));
        var second = document.AddLayer("B", new AssetReference(AssetId.New()));

        document.Selection.AddToSelection(first);
        document.Selection.AddToSelection(second, makeActive: true);

        var removed = document.RemoveNode(second);

        Assert.True(removed);
        Assert.False(document.Selection.IsSelected(second));
        Assert.True(document.Selection.IsSelected(first));
        Assert.Equal(first, document.Selection.ActiveNodeId);
    }

}
