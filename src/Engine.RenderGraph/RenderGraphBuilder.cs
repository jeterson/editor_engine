using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;

namespace Engine.RenderGraph;

public sealed class RenderGraphBuilder
{
    public RenderGraph Build(EditorDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var nodes = new List<RenderNode>();
        var executionOrder = new List<RenderNodeId>();
        var rootCompositeDependencies = new List<RenderNodeId>();
        var renderNodesByDocumentNode = new Dictionary<DocumentNodeId, List<RenderNodeId>>();

        foreach (var rootNode in document.EnumerateNodes())
        {
            var pipelineTail = BuildPipeline(rootNode, nodes, executionOrder, renderNodesByDocumentNode);
            rootCompositeDependencies.Add(pipelineTail);
        }

        var rootComposite = new CompositeRenderNode(RenderNodeId.New(), default, rootCompositeDependencies);
        nodes.Add(rootComposite);
        executionOrder.Add(rootComposite.Id);

        return new RenderGraph(nodes, executionOrder, renderNodesByDocumentNode.ToDictionary(x => x.Key, x => (IReadOnlyList<RenderNodeId>)x.Value));
    }

    private static RenderNodeId BuildPipeline(DocumentNode node, ICollection<RenderNode> nodes, ICollection<RenderNodeId> executionOrder, IDictionary<DocumentNodeId, List<RenderNodeId>> mapping)
        => node switch
        {
            Layer layer => BuildLayerPipeline(layer, nodes, executionOrder, mapping),
            LayerGroup group => BuildGroupPipeline(group, nodes, executionOrder, mapping),
            _ => throw new NotSupportedException($"Unsupported document node type '{node.GetType().Name}'.")
        };

    private static RenderNodeId BuildLayerPipeline(Layer layer, ICollection<RenderNode> nodes, ICollection<RenderNodeId> executionOrder, IDictionary<DocumentNodeId, List<RenderNodeId>> mapping)
    {
        var nodeIds = EnsureMappingBucket(mapping, layer.Id);

        var assetNode = new AssetRenderNode(RenderNodeId.New(), layer.AssetReference);
        nodeIds.Add(assetNode.Id);
        nodes.Add(assetNode);
        executionOrder.Add(assetNode.Id);

        var transformNode = new TransformRenderNode(RenderNodeId.New(), layer.Id, new[] { assetNode.Id });
        nodeIds.Add(transformNode.Id);
        nodes.Add(transformNode);
        executionOrder.Add(transformNode.Id);

        RenderNodeId current = transformNode.Id;
        foreach (var effect in layer.EffectStack.Effects.Where(effect => effect.IsEnabled))
        {
            var effectNode = new EffectRenderNode(RenderNodeId.New(), effect.Id, new[] { current });
            nodeIds.Add(effectNode.Id);
            nodes.Add(effectNode);
            executionOrder.Add(effectNode.Id);
            current = effectNode.Id;
        }

        var compositeNode = new CompositeRenderNode(RenderNodeId.New(), layer.Id, new[] { current });
        nodeIds.Add(compositeNode.Id);
        nodes.Add(compositeNode);
        executionOrder.Add(compositeNode.Id);
        return compositeNode.Id;
    }

    private static RenderNodeId BuildGroupPipeline(LayerGroup group, ICollection<RenderNode> nodes, ICollection<RenderNodeId> executionOrder, IDictionary<DocumentNodeId, List<RenderNodeId>> mapping)
    {
        var childDependencies = new List<RenderNodeId>();
        foreach (var child in group.Children)
        {
            childDependencies.Add(BuildPipeline(child, nodes, executionOrder, mapping));
        }

        var compositeNode = new CompositeRenderNode(RenderNodeId.New(), group.Id, childDependencies);
        EnsureMappingBucket(mapping, group.Id).Add(compositeNode.Id);
        nodes.Add(compositeNode);
        executionOrder.Add(compositeNode.Id);
        return compositeNode.Id;
    }

    private static List<RenderNodeId> EnsureMappingBucket(IDictionary<DocumentNodeId, List<RenderNodeId>> mapping, DocumentNodeId nodeId)
    {
        if (!mapping.TryGetValue(nodeId, out var nodeIds))
        {
            nodeIds = new List<RenderNodeId>();
            mapping[nodeId] = nodeIds;
        }

        return nodeIds;
    }
}
