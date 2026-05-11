using Engine.Domain.ValueObjects;

namespace Engine.RenderGraph;

public abstract record RenderNodeSemanticKey
{
    public sealed record Asset(AssetReference AssetReference) : RenderNodeSemanticKey;
    public sealed record Transform(DocumentNodeId DocumentNodeId) : RenderNodeSemanticKey;
    public sealed record Effect(DocumentNodeId DocumentNodeId, EffectId EffectId, string EffectType) : RenderNodeSemanticKey;
    public sealed record Composite(DocumentNodeId DocumentNodeId) : RenderNodeSemanticKey;
    public sealed record RootComposite : RenderNodeSemanticKey;
}
