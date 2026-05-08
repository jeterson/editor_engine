namespace Engine.Domain.ValueObjects;

/// <summary>
/// Identificador forte para um documento editável.
/// </summary>
public readonly record struct DocumentId(Guid Value)
{
    /// <summary>
    /// Cria um novo identificador único de documento.
    /// </summary>
    public static DocumentId New() => new(Guid.NewGuid());

    /// <summary>
    /// Tenta converter uma string para <see cref="DocumentId"/>.
    /// </summary>
    public static bool TryParse(string? value, out DocumentId id)
    {
        if (Guid.TryParse(value, out var parsed))
        {
            id = new DocumentId(parsed);
            return true;
        }

        id = default;
        return false;
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
