using Engine.Domain.ValueObjects;
using Engine.Infrastructure.Contracts;
using Engine.RenderGraph;

namespace Engine.Infrastructure.CPU;

public sealed class CpuAssetNodeProcessor : ICpuNodeProcessor
{
    private readonly IAssetResolver _assetResolver;

    public CpuAssetNodeProcessor(IAssetResolver assetResolver)
    {
        _assetResolver = assetResolver ?? throw new ArgumentNullException(nameof(assetResolver));
    }

    public bool CanProcess(RenderNode node) => node is AssetRenderNode;

    public ValueTask<CpuRenderSurface> ProcessAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken)
    {
        var assetNode = (AssetRenderNode)node;
        return ResolveToSurfaceAsync(assetNode.AssetReference, cancellationToken);
    }

    private async ValueTask<CpuRenderSurface> ResolveToSurfaceAsync(AssetReference assetReference, CancellationToken cancellationToken)
    {
        var decodedAsset = await _assetResolver.ResolveAsync(assetReference, cancellationToken);
        return CpuRenderSurface.FromDecodedAsset(decodedAsset);
    }
}

public sealed class CpuBrightnessNodeProcessor : ICpuNodeProcessor
{
    private readonly float _brightnessDelta;

    public CpuBrightnessNodeProcessor(float brightnessDelta = 0.2f)
    {
        _brightnessDelta = brightnessDelta;
    }

    public bool CanProcess(RenderNode node) => node is EffectRenderNode;

    public ValueTask<CpuRenderSurface> ProcessAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken)
    {
        var source = GetSingleDependencySurface(node, context).Clone();
        var pixels = source.GetWritablePixelSpan();

        for (var i = 0; i < pixels.Length; i += 4)
        {
            pixels[i] = Brighten(pixels[i]);
            pixels[i + 1] = Brighten(pixels[i + 1]);
            pixels[i + 2] = Brighten(pixels[i + 2]);
        }

        return ValueTask.FromResult(source);
    }

    private byte Brighten(byte component)
    {
        var value = component + (255f * _brightnessDelta);
        return (byte)Math.Clamp((int)MathF.Round(value), 0, 255);
    }

    private static CpuRenderSurface GetSingleDependencySurface(RenderNode node, RenderExecutionContext context)
    {
        if (node.Dependencies.Count != 1)
        {
            throw new InvalidOperationException($"Node '{node.Id}' requires exactly one dependency.");
        }

        if (!context.TryGetResult(node.Dependencies[0], out var sourceResult) || sourceResult?.Surface is not CpuRenderSurface source)
        {
            throw new InvalidOperationException($"Dependency surface for node '{node.Id}' is missing or incompatible.");
        }

        return source;
    }
}

public sealed class CpuCompositeNodeProcessor : ICpuNodeProcessor
{
    public bool CanProcess(RenderNode node) => node is CompositeRenderNode;

    public ValueTask<CpuRenderSurface> ProcessAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken)
    {
        if (node.Dependencies.Count == 0)
        {
            throw new InvalidOperationException("Composite node requires at least one dependency.");
        }

        var surfaces = node.Dependencies
            .Select(dependencyId => context.TryGetResult(dependencyId, out var result) ? result?.Surface as CpuRenderSurface : null)
            .ToList();

        if (surfaces.Any(surface => surface is null))
        {
            throw new InvalidOperationException("Composite node dependency surfaces must all be CPU surfaces.");
        }

        var baseSurface = surfaces[0]!.Clone();
        var destination = baseSurface.GetWritablePixelSpan();

        foreach (var overlay in surfaces.Skip(1))
        {
            var overlayPixels = overlay!.PixelBytes.Span;
            for (var i = 0; i < destination.Length; i += 4)
            {
                var alpha = overlayPixels[i + 3] / 255f;
                destination[i] = Blend(destination[i], overlayPixels[i], alpha);
                destination[i + 1] = Blend(destination[i + 1], overlayPixels[i + 1], alpha);
                destination[i + 2] = Blend(destination[i + 2], overlayPixels[i + 2], alpha);
                destination[i + 3] = (byte)Math.Clamp(destination[i + 3] + overlayPixels[i + 3], 0, 255);
            }
        }

        return ValueTask.FromResult(baseSurface);
    }

    private static byte Blend(byte dst, byte src, float alpha) => (byte)Math.Clamp((int)MathF.Round((dst * (1f - alpha)) + (src * alpha)), 0, 255);
}

public sealed class CpuTransformNodeProcessor : ICpuNodeProcessor
{
    public bool CanProcess(RenderNode node) => node is TransformRenderNode;

    public ValueTask<CpuRenderSurface> ProcessAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken)
    {
        if (node.Dependencies.Count != 1)
        {
            throw new InvalidOperationException("Transform node requires exactly one dependency in CPU baseline backend.");
        }

        if (!context.TryGetResult(node.Dependencies[0], out var dependency) || dependency?.Surface is not CpuRenderSurface source)
        {
            throw new InvalidOperationException("Transform node dependency is missing or incompatible.");
        }

        return ValueTask.FromResult(source.Clone());
    }
}
