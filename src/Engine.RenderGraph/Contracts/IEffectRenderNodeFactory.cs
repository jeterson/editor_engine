using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;

namespace Engine.RenderGraph.Contracts;

public interface IEffectRenderNodeFactory
{
    public Type EffectType { get; }
    RenderNode Create(Effect effect, DocumentNodeId sourceDocumentNodeId, RenderNodeId inputNodeId);
}
public interface IEffectRenderNodeFactory<TEffect> : IEffectRenderNodeFactory where TEffect : Effect
{
    RenderNode Create(TEffect effect, DocumentNodeId sourceDocumentNodeId, RenderNodeId inputNodeId);
}
public abstract class EffectRenderNodeFactory<TEffect> : IEffectRenderNodeFactory<TEffect> where TEffect : Effect
{
    public Type EffectType => typeof(TEffect);

    public abstract RenderNode Create(TEffect effect, DocumentNodeId sourceDocumentNodeId, RenderNodeId inputNodeId);

    public RenderNode Create(Effect effect, DocumentNodeId sourceDocumentNodeId, RenderNodeId inputNodeId)
    {
        return Create((TEffect)effect, sourceDocumentNodeId, inputNodeId);
    }
}
