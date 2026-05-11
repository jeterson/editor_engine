using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

public abstract record DocumentChange(DocumentNodeId NodeId);

public sealed record NodeAddedChange(DocumentNodeId NodeId) : DocumentChange(NodeId);
public sealed record NodeRemovedChange(DocumentNodeId NodeId) : DocumentChange(NodeId);
public sealed record NodeRenamedChange(DocumentNodeId NodeId) : DocumentChange(NodeId);
public sealed record TransformChangedChange(DocumentNodeId NodeId) : DocumentChange(NodeId);
public sealed record EffectChangedChange(DocumentNodeId NodeId, EffectId EffectId, string EffectType) : DocumentChange(NodeId);
public sealed record EffectParameterChangedChange(DocumentNodeId NodeId, EffectId EffectId, string EffectType, string? ParameterName = null) : DocumentChange(NodeId);
public sealed record VisibilityChangedChange(DocumentNodeId NodeId) : DocumentChange(NodeId);
