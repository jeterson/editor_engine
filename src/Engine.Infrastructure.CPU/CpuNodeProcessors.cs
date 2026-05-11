using Engine.Abstractions;
using Engine.Domain.ValueObjects;
using Engine.Infrastructure.Contracts;
using Engine.RenderGraph;
using Engine.RenderGraph.Abstractions;
using Engine.RenderGraph.Effects;

namespace Engine.Infrastructure.CPU;

public sealed class CpuAssetNodeProcessor : RenderNodeProcessor<AssetRenderNode>
{
    private readonly IAssetResolver _assetResolver;

    public CpuAssetNodeProcessor(IAssetResolver assetResolver)
    {
        _assetResolver = assetResolver ?? throw new ArgumentNullException(nameof(assetResolver));
    }

    public override ValueTask<IRenderSurface> ProcessAsync(AssetRenderNode node, RenderExecutionContext context, CancellationToken cancellationToken)
    {
        var assetNode = node;
        return ResolveToSurfaceAsync(assetNode.AssetReference, cancellationToken);
    }

    private async ValueTask<IRenderSurface> ResolveToSurfaceAsync(AssetReference assetReference, CancellationToken cancellationToken)
    {
        var decodedAsset = await _assetResolver.ResolveAsync(assetReference, cancellationToken);
        return CpuRenderSurface.FromDecodedAsset(decodedAsset);
    }
}

public sealed class CpuBrightnessNodeProcessor : RenderNodeProcessor<BrightnessRenderNode>
{
    public override ValueTask<IRenderSurface> ProcessAsync(BrightnessRenderNode node, RenderExecutionContext context, CancellationToken cancellationToken)
    {
        var source = GetSingleDependencySurface(node, context).Clone();

        var pixels = source.GetWritablePixelSpan();

        for (var i = 0; i < pixels.Length; i += 4)
        {
            pixels[i] = Brighten(pixels[i], node.Brightness);
            pixels[i + 1] = Brighten(pixels[i + 1], node.Brightness);
            pixels[i + 2] = Brighten(pixels[i + 2], node.Brightness);
        }

        return ValueTask.FromResult<IRenderSurface>(source);
    }

    private byte Brighten(byte component, float brightness)
    {
        var value = component + (255f * brightness);
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
public sealed class CpuCompositeNodeProcessor : RenderNodeProcessor<CompositeRenderNode>
{
    private static byte Blend(byte dst, byte src, float alpha) => (byte)Math.Clamp((int)MathF.Round((dst * (1f - alpha)) + (src * alpha)), 0, 255);

    public override ValueTask<IRenderSurface> ProcessAsync(CompositeRenderNode node, RenderExecutionContext context, CancellationToken cancellationToken)
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

        return ValueTask.FromResult<IRenderSurface>(baseSurface);
    }
}
public sealed class CpuTransformNodeProcessor : RenderNodeProcessor<TransformRenderNode>
{
    public override ValueTask<IRenderSurface> ProcessAsync(TransformRenderNode node, RenderExecutionContext context, CancellationToken cancellationToken)
    {
        if (node.Dependencies.Count != 1)
        {
            throw new InvalidOperationException("Transform node requires exactly one dependency in CPU baseline backend.");
        }

        if (!context.TryGetResult(node.Dependencies[0], out var dependency) || dependency?.Surface is not CpuRenderSurface source)
        {
            throw new InvalidOperationException("Transform node dependency is missing or incompatible.");
        }

        return ValueTask.FromResult<IRenderSurface>(ApplyTransform(source, node.Transform));
    }

    private static CpuRenderSurface ApplyTransform(CpuRenderSurface source, LayerTransform transform)
    {
        var normalizedRotation = NormalizeRotation(transform.RotationDegrees);
        return normalizedRotation switch
        {
            0 => source.Clone(),
            90 => Rotate90(source),
            180 => Rotate180(source),
            270 => Rotate270(source),
            _ => throw new NotSupportedException($"CPU baseline transform supports only 0, 90, 180 and 270 degrees. Received: {transform.RotationDegrees}.")
        };
    }

    private static int NormalizeRotation(double rotationDegrees)
    {
        var rounded = (int)Math.Round(rotationDegrees);
        var normalized = ((rounded % 360) + 360) % 360;
        return normalized;
    }

    private static CpuRenderSurface Rotate90(CpuRenderSurface source)
    {
        var srcWidth = source.Descriptor.Width;
        var srcHeight = source.Descriptor.Height;
        var result = new byte[source.PixelBytes.Length];
        var src = source.PixelBytes.Span;
        var dstWidth = srcHeight;

        for (var y = 0; y < srcHeight; y++)
        for (var x = 0; x < srcWidth; x++)
        {
            var dstX = srcHeight - 1 - y;
            var dstY = x;
            CopyPixel(src, srcWidth, x, y, result, dstWidth, dstX, dstY);
        }

        return new CpuRenderSurface(new RenderSurfaceDescriptor(srcHeight, srcWidth, source.Descriptor.PixelFormat, source.Descriptor.HasPremultipliedAlpha, RenderResourceLifetime.Transient), result);
    }

    private static CpuRenderSurface Rotate180(CpuRenderSurface source)
    {
        var srcWidth = source.Descriptor.Width;
        var srcHeight = source.Descriptor.Height;
        var result = new byte[source.PixelBytes.Length];
        var src = source.PixelBytes.Span;
        for (var y = 0; y < srcHeight; y++)
        for (var x = 0; x < srcWidth; x++)
        {
            var dstX = srcWidth - 1 - x;
            var dstY = srcHeight - 1 - y;
            CopyPixel(src, srcWidth, x, y, result, srcWidth, dstX, dstY);
        }

        return new CpuRenderSurface(source.Descriptor, result);
    }

    private static CpuRenderSurface Rotate270(CpuRenderSurface source)
    {
        var srcWidth = source.Descriptor.Width;
        var srcHeight = source.Descriptor.Height;
        var result = new byte[source.PixelBytes.Length];
        var src = source.PixelBytes.Span;
        var dstWidth = srcHeight;
        for (var y = 0; y < srcHeight; y++)
        for (var x = 0; x < srcWidth; x++)
        {
            var dstX = y;
            var dstY = srcWidth - 1 - x;
            CopyPixel(src, srcWidth, x, y, result, dstWidth, dstX, dstY);
        }

        return new CpuRenderSurface(new RenderSurfaceDescriptor(srcHeight, srcWidth, source.Descriptor.PixelFormat, source.Descriptor.HasPremultipliedAlpha, RenderResourceLifetime.Transient), result);
    }

    private static void CopyPixel(ReadOnlySpan<byte> src, int srcWidth, int srcX, int srcY, Span<byte> dst, int dstWidth, int dstX, int dstY)
    {
        var srcIndex = ((srcY * srcWidth) + srcX) * 4;
        var dstIndex = ((dstY * dstWidth) + dstX) * 4;
        dst[dstIndex] = src[srcIndex];
        dst[dstIndex + 1] = src[srcIndex + 1];
        dst[dstIndex + 2] = src[srcIndex + 2];
        dst[dstIndex + 3] = src[srcIndex + 3];
    }
}
