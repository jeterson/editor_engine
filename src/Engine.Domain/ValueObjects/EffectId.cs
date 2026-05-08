namespace Engine.Domain.ValueObjects;

/// <summary>
/// Identificador forte para um efeito aplicado na pilha de efeitos.
/// </summary>
public readonly record struct EffectId(Guid Value)
{
    /// <summary>
    /// Cria um novo identificador único de efeito.
    /// </summary>
    public static EffectId New() => new(Guid.NewGuid());

    /// <summary>
    /// Tenta converter uma string para <see cref="EffectId"/>.
    /// </summary>
    public static bool TryParse(string? value, out EffectId id)
    {
        if (Guid.TryParse(value, out var parsed))
        {
            id = new EffectId(parsed);
            return true;
        }

        id = default;
        return false;
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
