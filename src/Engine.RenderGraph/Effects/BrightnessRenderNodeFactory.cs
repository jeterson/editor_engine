using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Engine.RenderGraph.Contracts;

namespace Engine.RenderGraph.Effects;

public sealed class BrightnessRenderNodeFactory : EffectRenderNodeFactory<BrightnessEffect>
{
    public override RenderNode Create(BrightnessEffect effect, DocumentNodeId sourceDocumentNodeId, RenderNodeId inputNodeId)
    {
        return new BrightnessRenderNode(RenderNodeId.New(), sourceDocumentNodeId, effect.Id, inputNodeId, effect.Intensity);
    }
}
