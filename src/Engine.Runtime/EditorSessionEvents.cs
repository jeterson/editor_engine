using Engine.Domain.ValueObjects;
using Engine.RenderGraph;

namespace Engine.Runtime;

public sealed record RenderStarted(
    DateTimeOffset StartedAtUtc,
    IReadOnlyCollection<RenderNodeId> InvalidatedNodes,
    bool GraphRebuilt);

public sealed record RenderCompleted(
    DateTimeOffset StartedAtUtc,
    DateTimeOffset CompletedAtUtc,
    IReadOnlyDictionary<RenderNodeId, RenderResult> Results,
    IReadOnlyCollection<RenderNodeId> InvalidatedNodes,
    bool GraphRebuilt);

public sealed record RenderFailed(
    DateTimeOffset StartedAtUtc,
    DateTimeOffset FailedAtUtc,
    Exception Exception,
    IReadOnlyCollection<RenderNodeId> InvalidatedNodes,
    bool GraphRebuilt);

public sealed record PreviewUpdated(CurrentPreviewSurface Preview);
