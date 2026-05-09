using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Engine.RenderGraph.Contracts;

namespace Engine.RenderGraph.Effects;

public sealed class EffectRenderNodeFactoryRegistry
{
    private readonly Dictionary<Type, IEffectRenderNodeFactory> _factories;

    public EffectRenderNodeFactoryRegistry(IEnumerable<IEffectRenderNodeFactory> factories)
    {
        _factories = factories.ToDictionary(x => x.EffectType);
    }
    public RenderNode Create(Effect effect, RenderNodeId inputNodeId)
    {
        if (!_factories.TryGetValue(effect.GetType(), out var factory))
        {
            throw new InvalidOperationException(
                $"No render node factory registered for '{effect.GetType().Name}'.");
        }

        return factory.Create(effect, inputNodeId);
    }
}
