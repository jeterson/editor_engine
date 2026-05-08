namespace Engine.RenderGraph;

/// <summary>
/// Strongly typed identifier for render graph nodes.
/// </summary>
public readonly record struct RenderNodeId(Guid Value)
{
    public static RenderNodeId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
