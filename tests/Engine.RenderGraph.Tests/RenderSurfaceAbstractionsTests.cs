using Engine.Abstractions;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.RenderGraph.Tests;

public sealed class RenderSurfaceAbstractionsTests
{
    [Fact]
    public void RenderSurfaceDescriptor_Creates_WithExpectedValues()
    {
        var descriptor = new RenderSurfaceDescriptor(1920, 1080, PixelFormat.Rgba16Float, isHighPrecision: true, RenderResourceLifetime.Persistent);

        Assert.Equal(1920, descriptor.Width);
        Assert.Equal(1080, descriptor.Height);
        Assert.Equal(PixelFormat.Rgba16Float, descriptor.PixelFormat);
        Assert.True(descriptor.IsHighPrecision);
        Assert.Equal(RenderResourceLifetime.Persistent, descriptor.Lifetime);
    }

    [Fact]
    public void RenderSurfaceDescriptor_RejectsInvalidDimensions()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RenderSurfaceDescriptor(0, 10, PixelFormat.Rgba8, false, RenderResourceLifetime.Transient));
        Assert.Throws<ArgumentOutOfRangeException>(() => new RenderSurfaceDescriptor(10, -1, PixelFormat.Bgra8, false, RenderResourceLifetime.Frame));
    }

    [Fact]
    public void RenderResult_UsesTypedRenderSurface_AndRoundTripsThroughCache()
    {
        var nodeId = RenderNodeId.New();
        var descriptor = new RenderSurfaceDescriptor(256, 256, PixelFormat.Rgba8, false, RenderResourceLifetime.Frame);
        IRenderSurface surface = new FakeRenderSurface(descriptor);

        var result = new RenderResult(nodeId, surface);
        var key = new RenderCacheKey("k");
        var cached = result.ToCached(key, DateTimeOffset.UtcNow);
        var fromCache = RenderResult.FromCached(cached);

        Assert.Same(surface, result.Surface);
        Assert.Same(surface, cached.Surface);
        Assert.Same(surface, fromCache.Surface);
        Assert.Equal(nodeId, fromCache.NodeId);
    }

    [Fact]
    public void PixelFormat_AndLifetime_AreStableContractEnums()
    {
        Assert.Equal(0, (int)PixelFormat.Rgba8);
        Assert.Equal(1, (int)PixelFormat.Bgra8);
        Assert.Equal(2, (int)PixelFormat.Rgba16Float);

        Assert.Equal(0, (int)RenderResourceLifetime.Transient);
        Assert.Equal(1, (int)RenderResourceLifetime.Frame);
        Assert.Equal(2, (int)RenderResourceLifetime.Persistent);
    }

    private sealed record FakeRenderSurface(RenderSurfaceDescriptor Descriptor) : IRenderSurface;
}
