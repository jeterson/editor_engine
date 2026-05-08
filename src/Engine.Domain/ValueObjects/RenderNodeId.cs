namespace Engine.Domain.ValueObjects;

/// <summary>
/// Identificador forte para nós do grafo de renderização referenciados pelo domínio.
/// </summary>
public readonly record struct RenderNodeId(Guid Value)
{
    /// <summary>
    /// Cria um novo identificador único de nó de render.
    /// </summary>
    public static RenderNodeId New() => new(Guid.NewGuid());

    /// <summary>
    /// Tenta converter uma string para <see cref="RenderNodeId"/>.
    /// </summary>
    public static bool TryParse(string? value, out RenderNodeId id)
    {
        if (Guid.TryParse(value, out var parsed))
        {
            id = new RenderNodeId(parsed);
            return true;
        }

        id = default;
        return false;
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
