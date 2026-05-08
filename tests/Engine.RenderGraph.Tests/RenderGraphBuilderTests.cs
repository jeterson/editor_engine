using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.RenderGraph.Tests;

public sealed class RenderGraphBuilderTests
{
    [Fact]
    public void Build_GeneratesGraph_ForSingleLayer()
    {
        var doc = CreateDocument();
        doc.AddNode(new Layer(DocumentNodeId.New(), "Layer 1", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New())));

        var graph = new RenderGraphBuilder().Build(doc);

        Assert.NotEmpty(graph.Nodes);
        Assert.Equal(graph.Nodes.Count, graph.ExecutionOrder.Count);
        Assert.IsType<CompositeRenderNode>(graph.GetNode(graph.ExecutionOrder[^1]));
    }

    [Fact]
    public void Build_PreservesDependencyOrder()
    {
        var doc = CreateDocument();
        var layer = new Layer(DocumentNodeId.New(), "Layer", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
        doc.AddNode(layer);

        var graph = new RenderGraphBuilder().Build(doc);

        var positions = graph.ExecutionOrder.Select((id, index) => (id, index)).ToDictionary(x => x.id, x => x.index);

        foreach (var nodeId in graph.ExecutionOrder)
        {
            var node = graph.GetNode(nodeId);
            foreach (var dependency in node.Dependencies)
            {
                Assert.True(positions[dependency] < positions[nodeId]);
            }
        }
    }

    [Fact]
    public void Build_GeneratesGraph_ForGroupsAndLayers()
    {
        var doc = CreateDocument();
        var group = new LayerGroup(DocumentNodeId.New(), "Group");
        group.AddChild(new Layer(DocumentNodeId.New(), "L1", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New())));
        group.AddChild(new Layer(DocumentNodeId.New(), "L2", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New())));
        doc.AddNode(group);

        var graph = new RenderGraphBuilder().Build(doc);

        var compositeNodes = graph.Nodes.Values.OfType<CompositeRenderNode>().ToArray();
        Assert.True(compositeNodes.Length >= 3);
    }

    [Fact]
    public void Build_GeneratesEffectNodes_ForEnabledEffectsOnly()
    {
        var doc = CreateDocument();
        var layer = new Layer(DocumentNodeId.New(), "Layer", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New()));
        layer.EffectStack.Add(new BrightnessEffect(EffectId.New(), true, 10));
        layer.EffectStack.Add(new ContrastEffect(EffectId.New(), false, 5));
        doc.AddNode(layer);

        var graph = new RenderGraphBuilder().Build(doc);

        Assert.Single(graph.Nodes.Values.OfType<EffectRenderNode>());
    }

    [Fact]
    public void Build_TraversalIsConsistent_WithDocumentOrder()
    {
        var doc = CreateDocument();
        doc.AddNode(new Layer(DocumentNodeId.New(), "Bottom", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New())));
        doc.AddNode(new Layer(DocumentNodeId.New(), "Top", true, LayerTransform.Identity, Opacity.Opaque, BlendMode.Normal, new AssetReference(AssetId.New())));

        var graph = new RenderGraphBuilder().Build(doc);
        var layerCompositeNodes = graph.Nodes.Values.OfType<CompositeRenderNode>().Where(c => c.SourceDocumentNodeId != default).ToArray();

        Assert.True(layerCompositeNodes.Length >= 2);
    }

    private static EditorDocument CreateDocument() => new(DocumentId.New(), new CanvasSize(1920, 1080));
}
