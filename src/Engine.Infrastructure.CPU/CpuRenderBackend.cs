using Engine.RenderGraph;

namespace Engine.Infrastructure.CPU;

public sealed class CpuRenderBackend : IRenderBackend
{
    private readonly IReadOnlyList<ICpuNodeProcessor> _processors;

    public CpuRenderBackend(IEnumerable<ICpuNodeProcessor> processors)
    {
        _processors = processors?.ToList() ?? throw new ArgumentNullException(nameof(processors));
        if (_processors.Count == 0)
        {
            throw new ArgumentException("At least one processor is required.", nameof(processors));
        }
    }

    public async ValueTask<RenderResult> ExecuteNodeAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken)
    {
        var processor = _processors.FirstOrDefault(candidate => candidate.CanProcess(node));
        if (processor is null)
        {
            throw new InvalidOperationException($"No CPU processor registered for node type '{node.GetType().Name}'.");
        }

        var output = await processor.ProcessAsync(node, context, cancellationToken);
        return new RenderResult(node.Id, output);
    }
}
