using Engine.RenderGraph;
using Engine.RenderGraph.Abstractions;

namespace Engine.Infrastructure.CPU;

public sealed class CpuRenderBackend : IRenderBackend
{
    private readonly Dictionary<Type, IRenderNodeProcessor> _processors;

    public CpuRenderBackend(IEnumerable<IRenderNodeProcessor> processors)
    {
        _processors = processors.ToDictionary(x => x.NodeType);
        if (_processors.Count == 0)
        {
            throw new ArgumentException("At least one processor is required.", nameof(processors));
        }
    }

    public async ValueTask<RenderResult> ExecuteNodeAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken)
    {
        if (!_processors.TryGetValue(node.GetType(), out var processor))
        {
            throw new InvalidOperationException(
                $"No processor registered for '{node.GetType().Name}'.");
        }

        var output = await processor.ProcessAsync(node, context, cancellationToken);
        return new RenderResult(node.Id, output);
    }
}
