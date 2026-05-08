using Engine.Abstractions;

namespace Engine.RenderGraph;

/// <summary>
/// Backend-agnostic result produced by a render node execution.
/// </summary>
public sealed class RenderResult
{
    public RenderResult(RenderNodeId nodeId, IRenderSurface surface)
    {
        if (nodeId == default)
        {
            throw new ArgumentException("Node id must be non-default.", nameof(nodeId));
        }

        NodeId = nodeId;
        Surface = surface ?? throw new ArgumentNullException(nameof(surface));
    }

    public RenderNodeId NodeId { get; }

    public IRenderSurface Surface { get; }

    public CachedRenderResult ToCached(RenderCacheKey cacheKey, DateTimeOffset createdAtUtc)
    {
        return new CachedRenderResult(NodeId, cacheKey, createdAtUtc, Surface);
    }

    public static RenderResult FromCached(CachedRenderResult cached)
    {
        ArgumentNullException.ThrowIfNull(cached);
        return new RenderResult(cached.NodeId, cached.Surface);
    }
}
