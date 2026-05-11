using Engine.Domain.ValueObjects;

namespace Engine.RenderGraph;

public sealed class AssetRenderNode : RenderNode
{
    private readonly IReadOnlyList<KeyValuePair<string, string>> _cacheParameters;
    public AssetRenderNode(RenderNodeId id, AssetReference assetReference)
        : base(id, new RenderNodeSemanticKey.Asset(assetReference))
    {
        AssetReference = assetReference;
        _cacheParameters = new[]
        {
            new KeyValuePair<string, string>("asset", AssetReference.AssetId.Value.ToString())
        };
    }

    public AssetReference AssetReference { get; }

    public override IReadOnlyList<KeyValuePair<string, string>> GetCacheParameters() => _cacheParameters;
}

public sealed class TransformRenderNode : RenderNode
{
    private readonly IReadOnlyList<KeyValuePair<string, string>> _cacheParameters;
    public TransformRenderNode(RenderNodeId id, DocumentNodeId sourceDocumentNodeId, IReadOnlyCollection<RenderNodeId> dependencies)
        : base(id, new RenderNodeSemanticKey.Transform(sourceDocumentNodeId), dependencies)
    {
        SourceDocumentNodeId = sourceDocumentNodeId;
        _cacheParameters = new[]
        {
            new KeyValuePair<string, string>("source", SourceDocumentNodeId.Value.ToString())
        };
    }

    public DocumentNodeId SourceDocumentNodeId { get; }

    public override IReadOnlyList<KeyValuePair<string, string>> GetCacheParameters() => _cacheParameters;
}

public sealed class CompositeRenderNode : RenderNode
{
    private readonly IReadOnlyList<KeyValuePair<string, string>> _cacheParameters;
    public CompositeRenderNode(RenderNodeId id, DocumentNodeId sourceDocumentNodeId, IReadOnlyCollection<RenderNodeId> dependencies)
        : base(id, sourceDocumentNodeId == default ? new RenderNodeSemanticKey.RootComposite() : new RenderNodeSemanticKey.Composite(sourceDocumentNodeId), dependencies)
    {
        SourceDocumentNodeId = sourceDocumentNodeId;
        _cacheParameters = new[]
        {
            new KeyValuePair<string, string>("source", SourceDocumentNodeId.Value.ToString())
        };
    }

    public DocumentNodeId SourceDocumentNodeId { get; }

    public override IReadOnlyList<KeyValuePair<string, string>> GetCacheParameters() => _cacheParameters;
}
