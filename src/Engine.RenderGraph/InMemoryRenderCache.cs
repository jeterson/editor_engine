using Engine.RenderGraph.Abstractions;

namespace Engine.RenderGraph;

public sealed class InMemoryRenderCache : IRenderCache
{
    private readonly int _maxEntries;
    private readonly Dictionary<RenderCacheKey, LinkedListNode<CachedRenderResult>> _entries = new();
    private readonly LinkedList<CachedRenderResult> _lru = new();

    public InMemoryRenderCache(int maxEntries = 8)
    {
        _maxEntries = maxEntries;
    }

    public bool TryGet(RenderCacheKey key, out CachedRenderResult result)
    {
        if (_entries.TryGetValue(key, out var node))
        {
            _lru.Remove(node);
            _lru.AddFirst(node);

            result = node.Value;
            return true;
        }

        result = null!;
        return false;
    }

    public void Set(CachedRenderResult result)
    {
        if (_entries.TryGetValue(result.CacheKey, out var existing))
        {
            _lru.Remove(existing);
            _entries.Remove(result.CacheKey);
        }

        var node = new LinkedListNode<CachedRenderResult>(result);

        _lru.AddFirst(node);

        _entries[result.CacheKey] = node;

        EvictIfNeeded();
    }
    private void EvictIfNeeded()
    {
        while (_entries.Count > _maxEntries)
        {
            var last = _lru.Last;

            if (last is null)
            {
                return;
            }

            _lru.RemoveLast();

            _entries.Remove(last.Value.CacheKey);

            if (last.Value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
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
