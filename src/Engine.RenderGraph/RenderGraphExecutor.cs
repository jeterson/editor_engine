namespace Engine.RenderGraph;

public sealed class RenderGraphExecutor
{
    private readonly IRenderBackend _backend;

    public RenderGraphExecutor(IRenderBackend backend)
    {
        _backend = backend ?? throw new ArgumentNullException(nameof(backend));
    }

    public async ValueTask<IReadOnlyDictionary<RenderNodeId, RenderResult>> ExecuteAsync(
        RenderGraph graph,
        RenderExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(graph);
        ArgumentNullException.ThrowIfNull(context);

        var cacheKeysByNode = new Dictionary<RenderNodeId, RenderCacheKey>();

        foreach (var nodeId in graph.ExecutionOrder)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var node = graph.GetNode(nodeId);
            foreach (var dependencyId in node.Dependencies)
            {
                if (!context.TryGetResult(dependencyId, out _))
                {
                    throw new InvalidOperationException($"Dependency '{dependencyId}' must be executed before '{nodeId}'.");
                }
            }

            var key = RenderCacheKey.FromNode(node, cacheKeysByNode);
            cacheKeysByNode[nodeId] = key;

            var isInvalidated = context.InvalidatedNodes.Contains(nodeId);
            if (!isInvalidated && context.Cache.TryGet(key, out var cached))
            {
                context.SetResult(nodeId, RenderResult.FromCached(cached));
                continue;
            }

            if (isInvalidated)
            {
                context.Cache.Remove(key);
            }

            var result = await _backend.ExecuteNodeAsync(node, context, cancellationToken);
            var normalizedResult = result.NodeId == nodeId
                ? result
                : new RenderResult(nodeId, result.BackendPayload);

            context.SetResult(nodeId, normalizedResult);
            context.Cache.Set(normalizedResult.ToCached(key, DateTimeOffset.UtcNow));
        }

        return graph.ExecutionOrder.ToDictionary(nodeId => nodeId, nodeId =>
        {
            context.TryGetResult(nodeId, out var result);
            return result!;
        });
    }
}
