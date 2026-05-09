using Engine.Abstractions;

namespace Engine.RenderGraph.Abstractions;

public interface IRenderNodeProcessor
{
    Type NodeType { get; }

    ValueTask<IRenderSurface> ProcessAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken);
}
public interface IRenderNodeProcessor<in TNode> : IRenderNodeProcessor where TNode : RenderNode
{
    ValueTask<IRenderSurface> ProcessAsync(TNode node, RenderExecutionContext context, CancellationToken cancellationToken);
}
public abstract class RenderNodeProcessor<TNode> : IRenderNodeProcessor<TNode> where TNode : RenderNode
{
    public Type NodeType => typeof(TNode);

    public abstract ValueTask<IRenderSurface> ProcessAsync(TNode node, RenderExecutionContext context, CancellationToken cancellationToken);


    public ValueTask<IRenderSurface> ProcessAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken)
    {
        return ProcessAsync((TNode)node, context, cancellationToken);
    }
}

