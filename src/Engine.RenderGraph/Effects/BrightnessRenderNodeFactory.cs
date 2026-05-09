using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Engine.RenderGraph.Contracts;

namespace Engine.RenderGraph.Effects;

internal sealed class BrightnessRenderNodeFactory : EffectRenderNodeFactory<BrightnessEffect>
{
    public override RenderNode Create(BrightnessEffect effect, RenderNodeId inputNodeId)
    {
        return new BrightnessRenderNode(RenderNodeId.New(), inputNodeId, effect.Intensity);
    }
}
