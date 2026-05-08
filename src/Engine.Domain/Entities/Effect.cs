using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

/// <summary>
/// Representa uma instância configurada de uma operação visual declarativa e não destrutiva.
/// </summary>
public abstract class Effect
{
    protected Effect(EffectId id, bool isEnabled)
    {
        if (id == default)
        {
            throw new ArgumentException("Effect id must be non-default.", nameof(id));
        }

        Id = id;
        IsEnabled = isEnabled;
    }

    public EffectId Id { get; }

    public bool IsEnabled { get; private set; }

    public void SetEnabled(bool isEnabled) => IsEnabled = isEnabled;
}
