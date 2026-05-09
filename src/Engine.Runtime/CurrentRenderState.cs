using Engine.Domain.ValueObjects;

namespace Engine.Runtime;

public enum RenderLifecycleStatus
{
    Idle = 0,
    Rendering = 1,
    Completed = 2,
    Failed = 3
}

public sealed record CurrentRenderState(
    RenderLifecycleStatus Status,
    bool GraphRebuilt,
    IReadOnlyCollection<RenderNodeId> InvalidatedNodes,
    IReadOnlyCollection<RenderNodeId> ExecutedNodes,
    DateTimeOffset? StartedAtUtc,
    DateTimeOffset? CompletedAtUtc,
    string? FailureReason)
{
    public static CurrentRenderState Idle { get; } = new(
        RenderLifecycleStatus.Idle,
        GraphRebuilt: false,
        InvalidatedNodes: Array.Empty<RenderNodeId>(),
        ExecutedNodes: Array.Empty<RenderNodeId>(),
        StartedAtUtc: null,
        CompletedAtUtc: null,
        FailureReason: null);
}
