namespace Engine.RenderGraph.Abstractions;

public interface IRenderCache
{
    bool TryGet(RenderCacheKey key, out CachedRenderResult result);

    void Set(CachedRenderResult result);

    bool Remove(RenderCacheKey key);

    void Clear();
}
