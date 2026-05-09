using Engine.Domain.ValueObjects;

namespace Engine.RenderGraph.Effects;

public abstract class SingleInputEffectNode : RenderNode
{
    protected SingleInputEffectNode(RenderNodeId id, RenderNodeId inputNodeId) : base(id, [inputNodeId])
    {
        InputNodeId = inputNodeId;
    }
    public RenderNodeId InputNodeId { get; }
}
