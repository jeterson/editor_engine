using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.RenderGraph.Tests;

public sealed class DocumentChangeInvalidationTests
{
    [Fact]
    public void Build_ExposesDocumentToRenderMapping()
    {
        var layer = new Layer(DocumentNodeId.New(), "L1", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
        layer.EffectStack.Add(new BrightnessEffect(EffectId.New(), true, 0.25));
        var doc = new EditorDocument(DocumentId.New(), new CanvasSize(100, 100));
        doc.AddNode(layer);

        var graph = new RenderGraphBuilder().Build(doc);
        Assert.True(graph.RenderNodesByDocumentNode.TryGetValue(layer.Id, out var mapped));
        Assert.True(mapped.Count >= 4);
    }

    [Fact]
    public void InvalidateFromDocumentChanges_InvalidatesDependenciesAndAllowsPartial()
    {
        var left = new Layer(DocumentNodeId.New(), "L1", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
        var right = new Layer(DocumentNodeId.New(), "L2", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
        var doc = new EditorDocument(DocumentId.New(), new CanvasSize(100, 100));
        doc.AddNode(left);
        doc.AddNode(right);

        var graph = new RenderGraphBuilder().Build(doc);
        var manager = new RenderInvalidationManager(graph);

        var invalidated = manager.InvalidateFromDocumentChanges(new DocumentChange[] { new TransformChangedChange(left.Id) });

        Assert.NotEmpty(invalidated);
        Assert.True(graph.RenderNodesByDocumentNode[left.Id].Any(invalidated.Contains));
        Assert.False(graph.RenderNodesByDocumentNode[right.Id].All(invalidated.Contains));
    }
}
