using Engine.Domain.ValueObjects;
using System.Globalization;

namespace Engine.RenderGraph.Effects;

public class BrightnessRenderNode : SingleInputEffectNode
{
    private readonly IReadOnlyList<KeyValuePair<string, string>> _cacheParameters;

    public BrightnessRenderNode(RenderNodeId id, RenderNodeId inputNodeId, float brightness) : base(id, inputNodeId)
    {
        Brightness = brightness;

        _cacheParameters =
        [
            new("effect", "brightness"),
            new("brightness", brightness.ToString(CultureInfo.InvariantCulture))
        ];
    }
    public float Brightness { get; }

    public override IReadOnlyList<KeyValuePair<string, string>> GetCacheParameters()
        => _cacheParameters;
}
