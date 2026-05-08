namespace Engine.Domain.ValueObjects;

/// <summary>
/// Identificador forte para um asset de origem.
/// </summary>
public readonly record struct AssetId(Guid Value)
{
    /// <summary>
    /// Cria um novo identificador único de asset.
    /// </summary>
    public static AssetId New() => new(Guid.NewGuid());

    /// <summary>
    /// Tenta converter uma string para <see cref="AssetId"/>.
    /// </summary>
    public static bool TryParse(string? value, out AssetId id)
    {
        if (Guid.TryParse(value, out var parsed))
        {
            id = new AssetId(parsed);
            return true;
        }

        id = default;
        return false;
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
