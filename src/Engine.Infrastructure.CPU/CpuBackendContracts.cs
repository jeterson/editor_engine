using Engine.RenderGraph;

namespace Engine.Infrastructure.CPU;


public interface ICpuNodeProcessor
{
    bool CanProcess(RenderNode node);
    ValueTask<CpuRenderSurface> ProcessAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken);
}
