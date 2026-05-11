using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;

namespace Engine.RenderGraph;

public sealed class RenderInvalidationManager
{
    private readonly IReadOnlyDictionary<RenderNodeId, RenderNode> _nodes;
    private readonly Dictionary<RenderNodeId, IReadOnlyList<RenderNodeId>> _dependentsByNode;
    private readonly IReadOnlyDictionary<RenderNodeSemanticKey, IReadOnlyList<RenderNodeId>> _renderNodesBySemanticKey;
    private readonly IReadOnlyDictionary<DocumentNodeId, IReadOnlyList<RenderNodeId>> _renderNodesByDocumentNode;

    public RenderInvalidationManager(RenderGraph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);
        _nodes = graph.Nodes;
        _renderNodesBySemanticKey = BuildSemanticIndex(graph.Nodes.Values);
        _renderNodesByDocumentNode = graph.RenderNodesByDocumentNode;
        _dependentsByNode = BuildDependentsIndex(graph.Nodes.Values);
    }

    public IReadOnlyCollection<RenderNodeId> InvalidateFromDocumentChanges(IEnumerable<DocumentChange> changes)
    {
        ArgumentNullException.ThrowIfNull(changes);

        var invalidated = new HashSet<RenderNodeId>();
        foreach (var change in changes)
        {
            var mappedNodeIds = ResolveChangedNodes(change);

            foreach (var renderNodeId in mappedNodeIds)
            {
                foreach (var nodeId in InvalidateSubgraph(renderNodeId))
                {
                    invalidated.Add(nodeId);
                }
            }
        }

        return invalidated;
    }

    private IReadOnlyCollection<RenderNodeId> ResolveChangedNodes(DocumentChange change)
    {
        if (change is EffectParameterChangedChange effectParameterChange)
        {
            return GetMappedNodes(new RenderNodeSemanticKey.Effect(effectParameterChange.NodeId, effectParameterChange.EffectId, effectParameterChange.EffectType));
        }

        if (change is EffectChangedChange effectChange)
        {
            return GetMappedNodes(new RenderNodeSemanticKey.Effect(effectChange.NodeId, effectChange.EffectId, effectChange.EffectType));
        }

        if (change is TransformChangedChange)
            return GetMappedNodes(new RenderNodeSemanticKey.Transform(change.NodeId));
        if (change is VisibilityChangedChange)
            return GetMappedNodes(new RenderNodeSemanticKey.Composite(change.NodeId));

        return _renderNodesByDocumentNode.TryGetValue(change.NodeId, out var mappedNodeIds)
            ? mappedNodeIds
            : Array.Empty<RenderNodeId>();
    }

    private IReadOnlyCollection<RenderNodeId> GetMappedNodes(RenderNodeSemanticKey key)
        => _renderNodesBySemanticKey.TryGetValue(key, out var nodeIds) ? nodeIds : Array.Empty<RenderNodeId>();

    public IReadOnlyCollection<RenderNodeId> InvalidateSubgraph(RenderNodeId changedNodeId)
    {
        if (!_nodes.ContainsKey(changedNodeId)) throw new InvalidOperationException($"Cannot invalidate unknown node '{changedNodeId}'.");

        var invalidated = new HashSet<RenderNodeId>();
        var queue = new Queue<RenderNodeId>();
        queue.Enqueue(changedNodeId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!invalidated.Add(current)) continue;

            if (_dependentsByNode.TryGetValue(current, out var dependents))
            {
                foreach (var dependent in dependents) queue.Enqueue(dependent);
            }
        }

        return invalidated;
    }

    private static Dictionary<RenderNodeId, IReadOnlyList<RenderNodeId>> BuildDependentsIndex(IEnumerable<RenderNode> nodes)
    {
        var result = new Dictionary<RenderNodeId, List<RenderNodeId>>();
        foreach (var node in nodes)
        {
            if (!result.ContainsKey(node.Id)) result[node.Id] = new List<RenderNodeId>();
            foreach (var dependency in node.Dependencies)
            {
                if (!result.TryGetValue(dependency, out var dependents))
                {
                    dependents = new List<RenderNodeId>();
                    result[dependency] = dependents;
                }

                dependents.Add(node.Id);
            }
        }

        return result.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<RenderNodeId>)kvp.Value);
    }

    private static IReadOnlyDictionary<RenderNodeSemanticKey, IReadOnlyList<RenderNodeId>> BuildSemanticIndex(IEnumerable<RenderNode> nodes)
    {
        return nodes
            .GroupBy(x => x.SemanticKey)
            .ToDictionary(x => x.Key, x => (IReadOnlyList<RenderNodeId>)x.Select(y => y.Id).ToArray());
    }
}
