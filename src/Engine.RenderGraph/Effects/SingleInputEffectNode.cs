using Engine.Domain.ValueObjects;

namespace Engine.RenderGraph.Effects;

public abstract class SingleInputEffectNode : RenderNode
{
    protected SingleInputEffectNode(RenderNodeId id, RenderNodeSemanticKey semanticKey, RenderNodeId inputNodeId) : base(id, semanticKey, [inputNodeId])
    {
        InputNodeId = inputNodeId;
    }
    public RenderNodeId InputNodeId { get; }
}
