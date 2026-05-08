using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.RenderGraph.Tests;

public sealed class RenderCacheAndInvalidationTests
{
    [Fact]
    public void RenderCacheKey_IsDeterministic_ForSameOperation()
    {
        var assetNodeId = RenderNodeId.New();
        var assetNode = new AssetRenderNode(assetNodeId, new AssetReference(AssetId.New()));
        var assetKey = RenderCacheKey.FromNode(assetNode, new Dictionary<RenderNodeId, RenderCacheKey>());

        var transformNode = new TransformRenderNode(RenderNodeId.New(), DocumentNodeId.New(), new[] { assetNodeId });
        var dependencies = new Dictionary<RenderNodeId, RenderCacheKey> { [assetNodeId] = assetKey };

        var first = RenderCacheKey.FromNode(transformNode, dependencies);
        var second = RenderCacheKey.FromNode(transformNode, dependencies);

        Assert.Equal(first, second);
    }

    [Fact]
    public void RenderCacheKey_Changes_WhenDependenciesChange()
    {
        var dependencyId = RenderNodeId.New();
        var node = new CompositeRenderNode(RenderNodeId.New(), DocumentNodeId.New(), new[] { dependencyId });

        var keyA = RenderCacheKey.FromNode(node, new Dictionary<RenderNodeId, RenderCacheKey>
        {
            [dependencyId] = RenderCacheKey.FromNode(new AssetRenderNode(dependencyId, new AssetReference(AssetId.New())), new Dictionary<RenderNodeId, RenderCacheKey>())
        });

        var keyB = RenderCacheKey.FromNode(node, new Dictionary<RenderNodeId, RenderCacheKey>
        {
            [dependencyId] = RenderCacheKey.FromNode(new AssetRenderNode(dependencyId, new AssetReference(AssetId.New())), new Dictionary<RenderNodeId, RenderCacheKey>())
        });

        Assert.NotEqual(keyA, keyB);
    }

    [Fact]
    public void InMemoryRenderCache_SupportsHitAndMiss()
    {
        var cache = new InMemoryRenderCache();
        var node = new AssetRenderNode(RenderNodeId.New(), new AssetReference(AssetId.New()));
        var key = RenderCacheKey.FromNode(node, new Dictionary<RenderNodeId, RenderCacheKey>());

        Assert.False(cache.TryGet(key, out _));

        var expected = new CachedRenderResult(node.Id, key, DateTimeOffset.UtcNow);
        cache.Set(expected);

        Assert.True(cache.TryGet(key, out var actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RenderInvalidationManager_InvalidatesChangedNodeAndDependentsOnly()
    {
        var graph = CreateLinearGraph();
        var manager = new RenderInvalidationManager(graph);

        var middleNode = graph.ExecutionOrder[1];
        var invalidated = manager.InvalidateSubgraph(middleNode);

        Assert.Contains(middleNode, invalidated);
        Assert.Contains(graph.ExecutionOrder[2], invalidated);
        Assert.DoesNotContain(graph.ExecutionOrder[0], invalidated);
    }

    [Fact]
    public void RenderInvalidationManager_PreservesGraphConsistency_DuringPartialInvalidation()
    {
        var graph = CreateBranchedGraph();
        var manager = new RenderInvalidationManager(graph);

        var leftAsset = graph.ExecutionOrder[0];
        var invalidated = manager.InvalidateSubgraph(leftAsset).ToHashSet();

        Assert.Contains(leftAsset, invalidated);
        Assert.Contains(graph.ExecutionOrder[2], invalidated);
        Assert.Contains(graph.ExecutionOrder[4], invalidated);
        Assert.DoesNotContain(graph.ExecutionOrder[1], invalidated);
        Assert.DoesNotContain(graph.ExecutionOrder[3], invalidated);
    }

    private static RenderGraph CreateLinearGraph()
    {
        var n1 = new AssetRenderNode(RenderNodeId.New(), new AssetReference(AssetId.New()));
        var n2 = new TransformRenderNode(RenderNodeId.New(), DocumentNodeId.New(), new[] { n1.Id });
        var n3 = new CompositeRenderNode(RenderNodeId.New(), DocumentNodeId.New(), new[] { n2.Id });
        var nodes = new RenderNode[] { n1, n2, n3 };
        var order = new[] { n1.Id, n2.Id, n3.Id };
        return new RenderGraph(nodes, order);
    }

    private static RenderGraph CreateBranchedGraph()
    {
        var left = new AssetRenderNode(RenderNodeId.New(), new AssetReference(AssetId.New()));
        var right = new AssetRenderNode(RenderNodeId.New(), new AssetReference(AssetId.New()));
        var leftComposite = new CompositeRenderNode(RenderNodeId.New(), DocumentNodeId.New(), new[] { left.Id });
        var rightComposite = new CompositeRenderNode(RenderNodeId.New(), DocumentNodeId.New(), new[] { right.Id });
        var root = new CompositeRenderNode(RenderNodeId.New(), DocumentNodeId.New(), new[] { leftComposite.Id, rightComposite.Id });

        var nodes = new RenderNode[] { left, right, leftComposite, rightComposite, root };
        var order = new[] { left.Id, right.Id, leftComposite.Id, rightComposite.Id, root.Id };

        return new RenderGraph(nodes, order);
    }
}
