namespace Engine.RenderGraph.Abstractions;

/// <summary>
/// Abstraction that executes graph nodes without exposing graphics technology details.
/// </summary>
public interface IRenderBackend
{
    ValueTask<RenderResult> ExecuteNodeAsync(RenderNode node, RenderExecutionContext context, CancellationToken cancellationToken);
}
