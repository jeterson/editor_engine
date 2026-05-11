using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using System.Globalization;

namespace Engine.RenderGraph.Effects;

public class BrightnessRenderNode : SingleInputEffectNode
{
    private readonly IReadOnlyList<KeyValuePair<string, string>> _cacheParameters;

    public BrightnessRenderNode(RenderNodeId id, DocumentNodeId sourceDocumentNodeId, EffectId effectId, RenderNodeId inputNodeId, float brightness)
        : base(id, new RenderNodeSemanticKey.Effect(sourceDocumentNodeId, effectId, nameof(BrightnessEffect)), inputNodeId)
    {
        SourceDocumentNodeId = sourceDocumentNodeId;
        EffectId = effectId;
        Brightness = brightness;

        _cacheParameters =
        [
            new("effect", "brightness"),
            new("brightness", brightness.ToString(CultureInfo.InvariantCulture))
        ];
    }
    public DocumentNodeId SourceDocumentNodeId { get; }
    public EffectId EffectId { get; }
    public float Brightness { get; }

    public override IReadOnlyList<KeyValuePair<string, string>> GetCacheParameters()
        => _cacheParameters;
}
