using Engine.Domain.ValueObjects;
using Engine.RenderGraph;

namespace Engine.Infrastructure.CPU;

public interface IAssetResolver
{
    ValueTask<CpuRenderSurface> ResolveAsync(AssetReference assetReference, CancellationToken cancellationToken);
}

public interface ICpuNodeProcessor
{
    bool CanProcess(RenderNode node);
    ValueTask<CpuRenderSurface> ProcessAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken);
}
