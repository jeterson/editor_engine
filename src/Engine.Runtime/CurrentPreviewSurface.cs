using Engine.Abstractions;
using Engine.Domain.ValueObjects;

namespace Engine.Runtime;

public sealed record CurrentPreviewSurface(
    RenderNodeId NodeId,
    IRenderSurface Surface,
    DateTimeOffset UpdatedAtUtc);
