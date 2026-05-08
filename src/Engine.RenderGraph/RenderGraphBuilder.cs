using Engine.Domain.Entities;

namespace Engine.RenderGraph;

public sealed class RenderGraphBuilder
{
    public RenderGraph Build(EditorDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var nodes = new List<RenderNode>();
        var executionOrder = new List<RenderNodeId>();
        var rootCompositeDependencies = new List<RenderNodeId>();

        foreach (var rootNode in document.EnumerateNodes())
        {
            var pipelineTail = BuildPipeline(rootNode, nodes, executionOrder);
            rootCompositeDependencies.Add(pipelineTail);
        }

        var rootComposite = new CompositeRenderNode(RenderNodeId.New(), default, rootCompositeDependencies);
        nodes.Add(rootComposite);
        executionOrder.Add(rootComposite.Id);

        return new RenderGraph(nodes, executionOrder);
    }

    private static RenderNodeId BuildPipeline(DocumentNode node, ICollection<RenderNode> nodes, ICollection<RenderNodeId> executionOrder)
    {
        return node switch
        {
            Layer layer => BuildLayerPipeline(layer, nodes, executionOrder),
            LayerGroup group => BuildGroupPipeline(group, nodes, executionOrder),
            _ => throw new NotSupportedException($"Unsupported document node type '{node.GetType().Name}'.")
        };
    }

    private static RenderNodeId BuildLayerPipeline(Layer layer, ICollection<RenderNode> nodes, ICollection<RenderNodeId> executionOrder)
    {
        var assetNode = new AssetRenderNode(RenderNodeId.New(), layer.AssetReference);
        nodes.Add(assetNode);
        executionOrder.Add(assetNode.Id);

        var transformNode = new TransformRenderNode(RenderNodeId.New(), layer.Id, new[] { assetNode.Id });
        nodes.Add(transformNode);
        executionOrder.Add(transformNode.Id);

        RenderNodeId current = transformNode.Id;

        foreach (var effect in layer.EffectStack.Effects.Where(effect => effect.IsEnabled))
        {
            var effectNode = new EffectRenderNode(RenderNodeId.New(), effect.Id, new[] { current });
            nodes.Add(effectNode);
            executionOrder.Add(effectNode.Id);
            current = effectNode.Id;
        }

        var compositeNode = new CompositeRenderNode(RenderNodeId.New(), layer.Id, new[] { current });
        nodes.Add(compositeNode);
        executionOrder.Add(compositeNode.Id);

        return compositeNode.Id;
    }

    private static RenderNodeId BuildGroupPipeline(LayerGroup group, ICollection<RenderNode> nodes, ICollection<RenderNodeId> executionOrder)
    {
        var childDependencies = new List<RenderNodeId>();
        foreach (var child in group.Children)
        {
            childDependencies.Add(BuildPipeline(child, nodes, executionOrder));
        }

        var compositeNode = new CompositeRenderNode(RenderNodeId.New(), group.Id, childDependencies);
        nodes.Add(compositeNode);
        executionOrder.Add(compositeNode.Id);
        return compositeNode.Id;
    }
}
