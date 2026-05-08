namespace Engine.RenderGraph;

/// <summary>
/// Backend-agnostic executable operation in the render pipeline.
/// </summary>
public abstract class RenderNode
{
    private readonly List<RenderNodeId> _dependencies;

    protected RenderNode(RenderNodeId id, IReadOnlyCollection<RenderNodeId>? dependencies = null)
    {
        if (id == default)
        {
            throw new ArgumentException("Render node id must be non-default.", nameof(id));
        }

        Id = id;
        _dependencies = dependencies?.ToList() ?? new List<RenderNodeId>();

        if (_dependencies.Any(dependency => dependency == default))
        {
            throw new ArgumentException("Dependencies must not contain default ids.", nameof(dependencies));
        }
    }

    public RenderNodeId Id { get; }

    public IReadOnlyList<RenderNodeId> Dependencies => _dependencies;
}
