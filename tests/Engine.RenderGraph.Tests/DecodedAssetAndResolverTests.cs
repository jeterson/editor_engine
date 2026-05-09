using Engine.Abstractions;
using Engine.Domain.ValueObjects;
using Engine.Infrastructure.CPU;
using Xunit;

namespace Engine.RenderGraph.Tests;

public sealed class DecodedAssetAndResolverTests
{
    [Fact]
    public void DecodedAsset_CreatesImmutableSourceData()
    {
        var pixels = new byte[] { 1, 2, 3, 4 };
        var asset = new DecodedAsset(1, 1, PixelFormat.Rgba8, pixels);

        pixels[0] = 9;

        Assert.Equal(1, asset.Width);
        Assert.Equal(1, asset.Height);
        Assert.Equal(PixelFormat.Rgba8, asset.PixelFormat);
        Assert.Equal((byte)1, asset.PixelBytes.Span[0]);
    }

    [Fact]
    public void CpuRenderSurface_FromDecodedAsset_AdaptsSourceToRenderResource()
    {
        var asset = new DecodedAsset(1, 1, PixelFormat.Rgba8, new byte[] { 7, 8, 9, 10 });

        var surface = CpuRenderSurface.FromDecodedAsset(asset);

        Assert.Equal(1, surface.Descriptor.Width);
        Assert.Equal(1, surface.Descriptor.Height);
        Assert.Equal(PixelFormat.Rgba8, surface.Descriptor.PixelFormat);
        Assert.Equal(new byte[] { 7, 8, 9, 10 }, surface.PixelBytes.ToArray());
    }

    [Fact]
    public async Task AssetResolver_ReturnsDecodedAsset_AndProcessorConvertsToCpuSurface()
    {
        var reference = new AssetReference(AssetId.New());
        var resolver = new TestResolver();
        resolver.Set(reference, new DecodedAsset(1, 1, PixelFormat.Rgba8, new byte[] { 10, 20, 30, 40 }));

        var node = new AssetRenderNode(RenderNodeId.New(), reference);
        var processor = new CpuAssetNodeProcessor(resolver);

        var output = await processor.ProcessAsync(node, new RenderExecutionContext(new InMemoryRenderCache()), CancellationToken.None);

        Assert.IsType<CpuRenderSurface>(output);
        Assert.Equal(new byte[] { 10, 20, 30, 40 }, output.PixelBytes.ToArray());
    }

    private sealed class TestResolver : IAssetResolver
    {
        private readonly Dictionary<AssetReference, DecodedAsset> _assets = new();

        public void Set(AssetReference reference, DecodedAsset asset) => _assets[reference] = asset;

        public ValueTask<DecodedAsset> ResolveAsync(AssetReference assetReference, CancellationToken cancellationToken)
            => _assets.TryGetValue(assetReference, out var asset)
                ? ValueTask.FromResult(asset)
                : throw new InvalidOperationException("Asset not found");
    }
}
