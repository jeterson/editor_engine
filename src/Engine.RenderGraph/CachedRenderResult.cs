using Engine.Abstractions;
using Engine.Domain.ValueObjects;

namespace Engine.RenderGraph;

/// <summary>
/// Backend-agnostic cached render surface metadata.
/// </summary>
public sealed class CachedRenderResult : IDisposable
{
    private bool disposedValue;

    public CachedRenderResult(RenderNodeId nodeId, RenderCacheKey cacheKey, DateTimeOffset createdAtUtc, IRenderSurface surface)
    {
        if (nodeId == default)
        {
            throw new ArgumentException("Node id must be non-default.", nameof(nodeId));
        }

        NodeId = nodeId;
        CacheKey = cacheKey;
        CreatedAtUtc = createdAtUtc;
        Surface = surface ?? throw new ArgumentNullException(nameof(surface));
    }

    public RenderNodeId NodeId { get; }

    public RenderCacheKey CacheKey { get; }

    public DateTimeOffset CreatedAtUtc { get; }

    public IRenderSurface Surface { get; }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {

            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
