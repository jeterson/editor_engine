namespace Engine.RenderGraph;

public sealed class RenderInvalidationManager
{
    private readonly IReadOnlyDictionary<RenderNodeId, RenderNode> _nodes;
    private readonly Dictionary<RenderNodeId, IReadOnlyList<RenderNodeId>> _dependentsByNode;

    public RenderInvalidationManager(RenderGraph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);

        _nodes = graph.Nodes;
        _dependentsByNode = BuildDependentsIndex(graph.Nodes.Values);
    }

    public IReadOnlyCollection<RenderNodeId> InvalidateSubgraph(RenderNodeId changedNodeId)
    {
        if (!_nodes.ContainsKey(changedNodeId))
        {
            throw new InvalidOperationException($"Cannot invalidate unknown node '{changedNodeId}'.");
        }

        var invalidated = new HashSet<RenderNodeId>();
        var queue = new Queue<RenderNodeId>();
        queue.Enqueue(changedNodeId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!invalidated.Add(current))
            {
                continue;
            }

            if (_dependentsByNode.TryGetValue(current, out var dependents))
            {
                foreach (var dependent in dependents)
                {
                    queue.Enqueue(dependent);
                }
            }
        }

        return invalidated;
    }

    private static Dictionary<RenderNodeId, IReadOnlyList<RenderNodeId>> BuildDependentsIndex(IEnumerable<RenderNode> nodes)
    {
        var result = new Dictionary<RenderNodeId, List<RenderNodeId>>();

        foreach (var node in nodes)
        {
            if (!result.ContainsKey(node.Id))
            {
                result[node.Id] = new List<RenderNodeId>();
            }

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
}
