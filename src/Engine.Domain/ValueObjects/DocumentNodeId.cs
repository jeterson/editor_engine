namespace Engine.Domain.ValueObjects;

/// <summary>
/// Identificador forte para uma camada do documento.
/// </summary>
public readonly record struct DocumentNodeId(Guid Value)
{
    /// <summary>
    /// Cria um novo identificador único de camada.
    /// </summary>
    public static DocumentNodeId New() => new(Guid.NewGuid());

    /// <summary>
    /// Tenta converter uma string para <see cref="DocumentNodeId"/>.
    /// </summary>
    public static bool TryParse(string? value, out DocumentNodeId id)
    {
        if (Guid.TryParse(value, out var parsed))
        {
            id = new DocumentNodeId(parsed);
            return true;
        }

        id = default;
        return false;
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
