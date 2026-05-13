using Engine.Domain.ValueObjects;
using Engine.Infrastructure.Contracts;
using Engine.Infrastructure.CPU;
using Xunit;

namespace Engine.RenderGraph.Tests;

public sealed class CpuRenderBackendIntegrationTests
{
    [Fact]
    public async Task FullPipeline_AssetBrightnessComposite_WorksWithCache()
    {
        var asset1 = new AssetRenderNode(RenderNodeId.New(), new AssetReference(AssetId.New()));
        var asset2 = new AssetRenderNode(RenderNodeId.New(), new AssetReference(AssetId.New()));
        var bright = new EffectRenderNode(RenderNodeId.New(), EffectId.New(), new[] { asset1.Id });
        var composite = new CompositeRenderNode(RenderNodeId.New(), DocumentNodeId.New(), new[] { bright.Id, asset2.Id });

        var graph = new RenderGraph(new RenderNode[] { asset1, asset2, bright, composite }, new[] { asset1.Id, asset2.Id, bright.Id, composite.Id });
        var resolver = new InMemoryAssetResolver();
        resolver.Set(asset1.AssetReference, Surface(10, 20, 30, 255));
        resolver.Set(asset2.AssetReference, Surface(200, 0, 0, 128));

        var backend = new CpuRenderBackend(new ICpuNodeProcessor[]
        {
            new CpuAssetNodeProcessor(resolver),
            new CpuTransformNodeProcessor(),
            new CpuBrightnessNodeProcessor(0.1f),
            new CpuCompositeNodeProcessor()
        });

        var cache = new InMemoryRenderCache();
        var executor = new RenderGraphExecutor(backend);
        var first = await executor.ExecuteAsync(graph, new RenderExecutionContext(cache));
        var second = await executor.ExecuteAsync(graph, new RenderExecutionContext(cache));

        var output = Assert.IsType<CpuRenderSurface>(first[composite.Id].Surface);
        Assert.Equal(4, output.PixelBytes.Length);
        Assert.Equal(first[composite.Id].Surface, second[composite.Id].Surface);
    }

    [Fact]
    public void BrightnessProcessor_AdjustsPixelValues()
    {
        var sourceNode = new AssetRenderNode(RenderNodeId.New(), new AssetReference(AssetId.New()));
        var brightnessNode = new EffectRenderNode(RenderNodeId.New(), EffectId.New(), new[] { sourceNode.Id });
        var sourceSurface = Surface(10, 20, 30, 255);
        var context = new RenderExecutionContext(new InMemoryRenderCache());
        context.SetResult(sourceNode.Id, new RenderResult(sourceNode.Id, sourceSurface));

        var processor = new CpuBrightnessNodeProcessor(0.1f);
        var rendered = processor.ProcessAsync(brightnessNode, context, CancellationToken.None).Result;

        Assert.Equal(new byte[] { 36, 46, 56, 255 }, rendered.PixelBytes.ToArray());
    }

    [Fact]
    public void CompositeProcessor_ComposesTwoSurfaces()
    {
        var a = new AssetRenderNode(RenderNodeId.New(), new AssetReference(AssetId.New()));
        var b = new AssetRenderNode(RenderNodeId.New(), new AssetReference(AssetId.New()));
        var compositeNode = new CompositeRenderNode(RenderNodeId.New(), DocumentNodeId.New(), new[] { a.Id, b.Id });
        var context = new RenderExecutionContext(new InMemoryRenderCache());
        context.SetResult(a.Id, new RenderResult(a.Id, Surface(0, 0, 0, 255)));
        context.SetResult(b.Id, new RenderResult(b.Id, Surface(255, 0, 0, 128)));

        var output = new CpuCompositeNodeProcessor().ProcessAsync(compositeNode, context, CancellationToken.None).Result;
        Assert.True(output.PixelBytes.Span[0] is > 120 and < 135);
    }

    [Fact]
    public void CpuRenderSurface_IsValidRenderSurface()
    {
        var surface = Surface(1, 2, 3, 4);
        Assert.Equal(1, surface.Descriptor.Width);
        Assert.Equal(1, surface.Descriptor.Height);
        Assert.Equal(4, surface.PixelBytes.Length);
    }

    [Fact]
    public void TransformProcessor_Rotates90Clockwise_WithoutMutatingInput()
    {
        var srcNode = new AssetRenderNode(RenderNodeId.New(), new AssetReference(AssetId.New()));
        var transformNode = new TransformRenderNode(RenderNodeId.New(), DocumentNodeId.New(), new LayerTransform(0, 0, 1, 1, 90), new[] { srcNode.Id });
        var source = Surface2x1((255, 0, 0, 255), (0, 255, 0, 255));
        var sourceSnapshot = source.PixelBytes.ToArray();
        var context = new RenderExecutionContext(new InMemoryRenderCache());
        context.SetResult(srcNode.Id, new RenderResult(srcNode.Id, source));

        var output = Assert.IsType<CpuRenderSurface>(new CpuTransformNodeProcessor().ProcessAsync(transformNode, context, CancellationToken.None).Result);

        Assert.Equal(sourceSnapshot, source.PixelBytes.ToArray());
        Assert.Equal(1, output.Descriptor.Width);
        Assert.Equal(2, output.Descriptor.Height);
        Assert.Equal(new byte[] { 255, 0, 0, 255, 0, 255, 0, 255 }, output.PixelBytes.ToArray());
    }

    private static CpuRenderSurface Surface(byte r, byte g, byte b, byte a)
        => new(new Engine.Abstractions.RenderSurfaceDescriptor(1, 1, Engine.Abstractions.PixelFormat.Rgba8, false, Engine.Abstractions.RenderResourceLifetime.Transient), new[] { r, g, b, a });

    private static CpuRenderSurface Surface2x1((byte r, byte g, byte b, byte a) left, (byte r, byte g, byte b, byte a) right)
        => new(new Engine.Abstractions.RenderSurfaceDescriptor(2, 1, Engine.Abstractions.PixelFormat.Rgba8, false, Engine.Abstractions.RenderResourceLifetime.Transient), new[]
        {
            left.r, left.g, left.b, left.a,
            right.r, right.g, right.b, right.a
        });

    private sealed class InMemoryAssetResolver : IAssetResolver
    {
        private readonly Dictionary<AssetReference, DecodedAsset> _assets = new();

        public void Set(AssetReference reference, CpuRenderSurface surface)
            => _assets[reference] = new DecodedAsset(surface.Descriptor.Width, surface.Descriptor.Height, surface.Descriptor.PixelFormat, surface.PixelBytes.ToArray());

        public ValueTask<DecodedAsset> ResolveAsync(AssetReference assetReference, CancellationToken cancellationToken)
            => _assets.TryGetValue(assetReference, out var asset)
                ? ValueTask.FromResult(asset)
                : throw new InvalidOperationException($"Asset not found: {assetReference.AssetId.Value}");
    }
}
