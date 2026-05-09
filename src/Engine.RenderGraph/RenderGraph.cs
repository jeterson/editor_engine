using Engine.Domain.ValueObjects;

namespace Engine.RenderGraph;

public sealed class RenderGraph
{
    private readonly Dictionary<RenderNodeId, RenderNode> _nodes;
    private readonly Dictionary<DocumentNodeId, IReadOnlyList<RenderNodeId>> _renderNodesByDocumentNode;

    public RenderGraph(
        IEnumerable<RenderNode> nodes,
        IReadOnlyList<RenderNodeId> executionOrder,
        IReadOnlyDictionary<DocumentNodeId, IReadOnlyList<RenderNodeId>>? renderNodesByDocumentNode = null)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        ArgumentNullException.ThrowIfNull(executionOrder);

        _nodes = nodes.ToDictionary(node => node.Id);
        ExecutionOrder = executionOrder;
        _renderNodesByDocumentNode = renderNodesByDocumentNode is null
            ? new Dictionary<DocumentNodeId, IReadOnlyList<RenderNodeId>>()
            : new Dictionary<DocumentNodeId, IReadOnlyList<RenderNodeId>>(renderNodesByDocumentNode);

        ValidateGraph();
    }

    public IReadOnlyDictionary<RenderNodeId, RenderNode> Nodes => _nodes;

    public IReadOnlyList<RenderNodeId> ExecutionOrder { get; }
    public IReadOnlyDictionary<DocumentNodeId, IReadOnlyList<RenderNodeId>> RenderNodesByDocumentNode => _renderNodesByDocumentNode;

    public RenderNode GetNode(RenderNodeId id) => _nodes[id];

    private void ValidateGraph()
    {
        if (_nodes.Count != ExecutionOrder.Count)
        {
            throw new InvalidOperationException("Execution order must contain all graph nodes exactly once.");
        }

        var visited = new HashSet<RenderNodeId>();
        foreach (var nodeId in ExecutionOrder)
        {
            if (!_nodes.ContainsKey(nodeId))
            {
                throw new InvalidOperationException($"Execution order references unknown node '{nodeId}'.");
            }

            if (!visited.Add(nodeId))
            {
                throw new InvalidOperationException($"Execution order contains duplicated node '{nodeId}'.");
            }

            var node = _nodes[nodeId];
            foreach (var dependencyId in node.Dependencies)
            {
                if (!_nodes.ContainsKey(dependencyId))
                {
                    throw new InvalidOperationException($"Node '{nodeId}' references unknown dependency '{dependencyId}'.");
                }

                if (!visited.Contains(dependencyId))
                {
                    throw new InvalidOperationException($"Node '{nodeId}' is scheduled before dependency '{dependencyId}'.");
                }
            }
        }
    }
}
