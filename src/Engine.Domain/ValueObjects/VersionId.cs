namespace Engine.Domain.ValueObjects;

/// <summary>
/// Identificador forte para versões imutáveis de estado do documento.
/// </summary>
public readonly record struct VersionId(Guid Value)
{
    /// <summary>
    /// Cria um novo identificador único de versão.
    /// </summary>
    public static VersionId New() => new(Guid.NewGuid());

    /// <summary>
    /// Tenta converter uma string para <see cref="VersionId"/>.
    /// </summary>
    public static bool TryParse(string? value, out VersionId id)
    {
        if (Guid.TryParse(value, out var parsed))
        {
            id = new VersionId(parsed);
            return true;
        }

        id = default;
        return false;
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
