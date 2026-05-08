using Engine.Domain.ValueObjects;

namespace Engine.RenderGraph;

public sealed class AssetRenderNode : RenderNode
{
    public AssetRenderNode(RenderNodeId id, AssetReference assetReference)
        : base(id)
    {
        AssetReference = assetReference;
    }

    public AssetReference AssetReference { get; }
}

public sealed class TransformRenderNode : RenderNode
{
    public TransformRenderNode(RenderNodeId id, DocumentNodeId sourceDocumentNodeId, IReadOnlyCollection<RenderNodeId> dependencies)
        : base(id, dependencies)
    {
        SourceDocumentNodeId = sourceDocumentNodeId;
    }

    public DocumentNodeId SourceDocumentNodeId { get; }
}

public sealed class EffectRenderNode : RenderNode
{
    public EffectRenderNode(RenderNodeId id, EffectId effectId, IReadOnlyCollection<RenderNodeId> dependencies)
        : base(id, dependencies)
    {
        EffectId = effectId;
    }

    public EffectId EffectId { get; }
}

public sealed class CompositeRenderNode : RenderNode
{
    public CompositeRenderNode(RenderNodeId id, DocumentNodeId sourceDocumentNodeId, IReadOnlyCollection<RenderNodeId> dependencies)
        : base(id, dependencies)
    {
        SourceDocumentNodeId = sourceDocumentNodeId;
    }

    public DocumentNodeId SourceDocumentNodeId { get; }
}
