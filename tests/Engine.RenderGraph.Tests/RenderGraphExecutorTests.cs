using Engine.Abstractions;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.RenderGraph.Tests;

public sealed class RenderGraphExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_ExecutesGraph_InDependencyOrder()
    {
        var graph = CreateLinearGraph();
        var backend = new FakeBackend();
        var executor = new RenderGraphExecutor(backend);
        var context = new RenderExecutionContext(new InMemoryRenderCache());

        var results = await executor.ExecuteAsync(graph, context);

        Assert.Equal(graph.ExecutionOrder, backend.ExecutedNodeIds);
        Assert.Equal(graph.Nodes.Count, results.Count);
    }

    [Fact]
    public async Task ExecuteAsync_UsesCacheHit_AndSkipsBackendExecution()
    {
        var graph = CreateLinearGraph();
        var cache = new InMemoryRenderCache();
        var backend = new FakeBackend();
        var executor = new RenderGraphExecutor(backend);

        await executor.ExecuteAsync(graph, new RenderExecutionContext(cache));
        backend.Reset();

        await executor.ExecuteAsync(graph, new RenderExecutionContext(cache));

        Assert.Empty(backend.ExecutedNodeIds);
    }

    [Fact]
    public async Task ExecuteAsync_RespectsInvalidation_AndReexecutesAffectedNodes()
    {
        var graph = CreateLinearGraph();
        var cache = new InMemoryRenderCache();
        var backend = new FakeBackend();
        var executor = new RenderGraphExecutor(backend);

        await executor.ExecuteAsync(graph, new RenderExecutionContext(cache));
        backend.Reset();

        var invalidation = new RenderInvalidationManager(graph).InvalidateSubgraph(graph.ExecutionOrder[1]);
        await executor.ExecuteAsync(graph, new RenderExecutionContext(cache, invalidation));

        Assert.Equal(new[] { graph.ExecutionOrder[1], graph.ExecutionOrder[2] }, backend.ExecutedNodeIds);
    }

    [Fact]
    public async Task ExecuteAsync_IntegratesExecutorCacheAndBackend()
    {
        var graph = CreateLinearGraph();
        var cache = new InMemoryRenderCache();
        var backend = new FakeBackend();
        var executor = new RenderGraphExecutor(backend);

        var results = await executor.ExecuteAsync(graph, new RenderExecutionContext(cache));

        var dependencyKeys = new Dictionary<RenderNodeId, RenderCacheKey>();
        foreach (var nodeId in graph.ExecutionOrder)
        {
            var node = graph.GetNode(nodeId);
            var key = RenderCacheKey.FromNode(node, dependencyKeys);
            dependencyKeys[nodeId] = key;

            Assert.True(cache.TryGet(key, out var cached));
            Assert.Equal(nodeId, cached.NodeId);
            Assert.True(results.ContainsKey(nodeId));
        }
    }

    private static RenderGraph CreateLinearGraph()
    {
        var n1 = new AssetRenderNode(RenderNodeId.New(), new AssetReference(AssetId.New()));
        var n2 = new TransformRenderNode(RenderNodeId.New(), DocumentNodeId.New(), new[] { n1.Id });
        var n3 = new CompositeRenderNode(RenderNodeId.New(), DocumentNodeId.New(), new[] { n2.Id });
        return new RenderGraph(new RenderNode[] { n1, n2, n3 }, new[] { n1.Id, n2.Id, n3.Id });
    }

    private sealed class FakeBackend : IRenderBackend
    {
        public List<RenderNodeId> ExecutedNodeIds { get; } = new();

        public ValueTask<RenderResult> ExecuteNodeAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken)
        {
            ExecutedNodeIds.Add(node.Id);
            var descriptor = new RenderSurfaceDescriptor(64, 64, PixelFormat.Rgba8, isHighPrecision: false, RenderResourceLifetime.Transient);
            return ValueTask.FromResult(new RenderResult(node.Id, new FakeRenderSurface(descriptor)));
        }

        public void Reset() => ExecutedNodeIds.Clear();

        private sealed record FakeRenderSurface(RenderSurfaceDescriptor Descriptor) : IRenderSurface;
    }
}
