using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

/// <summary>
/// Pilha ordenada de efeitos declarativos de uma layer.
/// </summary>
public sealed class EffectStack
{
    private readonly List<LayerEffect> _effects;

    public EffectStack()
    {
        _effects = new List<LayerEffect>();
    }

    public IReadOnlyList<LayerEffect> Effects => _effects;

    public void Add(LayerEffect effect)
    {
        ArgumentNullException.ThrowIfNull(effect);

        if (_effects.Any(existing => existing.Id == effect.Id))
        {
            throw new InvalidOperationException($"Effect with id '{effect.Id}' already exists in stack.");
        }

        _effects.Add(effect);
    }

    public void Remove(EffectId effectId)
    {
        if (effectId == default)
        {
            throw new ArgumentException("Effect id must be non-default.", nameof(effectId));
        }

        var index = FindIndexById(effectId);
        _effects.RemoveAt(index);
    }

    public void Reorder(EffectId effectId, int targetIndex)
    {
        if (effectId == default)
        {
            throw new ArgumentException("Effect id must be non-default.", nameof(effectId));
        }

        if (targetIndex < 0 || targetIndex >= _effects.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(targetIndex), "Target index is outside effect stack bounds.");
        }

        var currentIndex = FindIndexById(effectId);
        if (currentIndex == targetIndex)
        {
            return;
        }

        var effect = _effects[currentIndex];
        _effects.RemoveAt(currentIndex);
        _effects.Insert(targetIndex, effect);
    }

    public void SetEnabled(EffectId effectId, bool isEnabled)
    {
        if (effectId == default)
        {
            throw new ArgumentException("Effect id must be non-default.", nameof(effectId));
        }

        var index = FindIndexById(effectId);
        _effects[index].SetEnabled(isEnabled);
    }

    private int FindIndexById(EffectId effectId)
    {
        var index = _effects.FindIndex(effect => effect.Id == effectId);
        if (index < 0)
        {
            throw new InvalidOperationException($"Effect with id '{effectId}' does not exist in stack.");
        }

        return index;
    }
}
