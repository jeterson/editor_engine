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
