namespace Engine.RenderGraph;

/// <summary>
/// Backend-agnostic result produced by a render node execution.
/// Carries an optional payload token that concrete infrastructure backends may interpret.
/// </summary>
public sealed class RenderResult
{
    public RenderResult(RenderNodeId nodeId, object? backendPayload = null)
    {
        if (nodeId == default)
        {
            throw new ArgumentException("Node id must be non-default.", nameof(nodeId));
        }

        NodeId = nodeId;
        BackendPayload = backendPayload;
    }

    public RenderNodeId NodeId { get; }

    public object? BackendPayload { get; }

    public CachedRenderResult ToCached(RenderCacheKey cacheKey, DateTimeOffset createdAtUtc)
    {
        return new CachedRenderResult(NodeId, cacheKey, createdAtUtc, BackendPayload);
    }

    public static RenderResult FromCached(CachedRenderResult cached)
    {
        ArgumentNullException.ThrowIfNull(cached);
        return new RenderResult(cached.NodeId, cached.BackendPayload);
    }
}
