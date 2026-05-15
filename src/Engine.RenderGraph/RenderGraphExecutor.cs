using Engine.Abstractions.Observability;
using Engine.Domain.ValueObjects;
using Engine.RenderGraph.Abstractions;
using System.Diagnostics;

namespace Engine.RenderGraph;

public sealed class RenderGraphExecutor
{
    private readonly IRenderBackend _backend;
    private readonly IRenderLogger _renderLogger;

    public RenderGraphExecutor(IRenderBackend backend, IRenderLogger renderLogger)
    {
        _backend = backend ?? throw new ArgumentNullException(nameof(backend));
        _renderLogger = renderLogger;
    }

    public async ValueTask<IReadOnlyDictionary<RenderNodeId, RenderResult>> ExecuteAsync(
        RenderGraph graph,
        RenderExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(graph);
        ArgumentNullException.ThrowIfNull(context);

        var cacheKeysByNode = new Dictionary<RenderNodeId, RenderCacheKey>();
        var stopWatchExecutor = new Stopwatch();
        var stopWatchNodeExecutor = new Stopwatch();

        _renderLogger.ExecutionStarted(graph.ExecutionOrder.Count);

        foreach (var nodeId in graph.ExecutionOrder)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var node = graph.GetNode(nodeId);

            stopWatchNodeExecutor.Restart();

            _renderLogger.NodeExecutionStart(nodeId, node.GetType().Name);

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
                _renderLogger.NodeCacheHit(nodeId);
                context.SetResult(nodeId, RenderResult.FromCached(cached));
                continue;
            }

            if (isInvalidated)
            {
                context.Cache.Remove(key);
            }

            _renderLogger.NodeCacheMiss(nodeId);

            var result = await _backend.ExecuteNodeAsync(node, context, cancellationToken);
            var normalizedResult = result.NodeId == nodeId
                ? result
                : new RenderResult(nodeId, result.Surface);

            context.SetResult(nodeId, normalizedResult);
            context.Cache.Set(normalizedResult.ToCached(key, DateTimeOffset.UtcNow));
            _renderLogger.NodeExecutionCompleted(nodeId, node.GetType().Name, stopWatchNodeExecutor.Elapsed);
        }

        _renderLogger.ExecutionCompleted(graph.ExecutionOrder.Count, stopWatchExecutor.Elapsed);

        return graph.ExecutionOrder.ToDictionary(nodeId => nodeId, nodeId =>
        {
            context.TryGetResult(nodeId, out var result);
            return result!;
        });
    }
}
