using Engine.RenderGraph.Abstractions;

namespace Engine.RenderGraph;

public sealed class InMemoryRenderCache : IRenderCache
{
    private readonly Dictionary<RenderCacheKey, CachedRenderResult> _entries = new();

    public bool TryGet(RenderCacheKey key, out CachedRenderResult result)
    {
        return _entries.TryGetValue(key, out result!);
    }

    public void Set(CachedRenderResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        _entries[result.CacheKey] = result;
    }

    public bool Remove(RenderCacheKey key)
    {
        return _entries.Remove(key);
    }

    public void Clear()
    {
        _entries.Clear();
    }
}
