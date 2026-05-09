using Engine.Domain.ValueObjects;
using Engine.RenderGraph.Abstractions;

namespace Engine.RenderGraph;

/// <summary>
/// Execution-scoped state used by the executor and backend.
/// </summary>
public sealed class RenderExecutionContext
{
    private readonly Dictionary<RenderNodeId, RenderResult> _resultsByNode = new();

    public RenderExecutionContext(IRenderCache cache, IReadOnlyCollection<RenderNodeId>? invalidatedNodes = null)
    {
        Cache = cache ?? throw new ArgumentNullException(nameof(cache));
        InvalidatedNodes = invalidatedNodes?.ToHashSet() ?? new HashSet<RenderNodeId>();
    }

    public IRenderCache Cache { get; }

    public IReadOnlySet<RenderNodeId> InvalidatedNodes { get; }

    public bool TryGetResult(RenderNodeId nodeId, out RenderResult result)
    {
        return _resultsByNode.TryGetValue(nodeId, out result!);
    }

    public void SetResult(RenderNodeId nodeId, RenderResult result)
    {
        if (nodeId == default)
        {
            throw new ArgumentException("Node id must be non-default.", nameof(nodeId));
        }

        ArgumentNullException.ThrowIfNull(result);
        _resultsByNode[nodeId] = result;
    }
}
