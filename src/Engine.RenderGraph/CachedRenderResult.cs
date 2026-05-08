namespace Engine.RenderGraph;

/// <summary>
/// Backend-agnostic cached render payload. Real surfaces/resources are intentionally deferred to infrastructure backends.
/// </summary>
public sealed class CachedRenderResult
{
    public CachedRenderResult(RenderNodeId nodeId, RenderCacheKey cacheKey, DateTimeOffset createdAtUtc, object? backendPayload = null)
    {
        if (nodeId == default)
        {
            throw new ArgumentException("Node id must be non-default.", nameof(nodeId));
        }

        NodeId = nodeId;
        CacheKey = cacheKey;
        CreatedAtUtc = createdAtUtc;
        BackendPayload = backendPayload;
    }

    public RenderNodeId NodeId { get; }

    public RenderCacheKey CacheKey { get; }

    public DateTimeOffset CreatedAtUtc { get; }

    /// <summary>
    /// Optional future hook for backend-specific payload types (CPU buffer, GPU texture handle wrapper, etc.).
    /// </summary>
    public object? BackendPayload { get; }
}
