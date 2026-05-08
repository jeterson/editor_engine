namespace Engine.Domain.ValueObjects;

/// <summary>
/// Identificador forte para uma camada do documento.
/// </summary>
public readonly record struct LayerId(Guid Value)
{
    /// <summary>
    /// Cria um novo identificador único de camada.
    /// </summary>
    public static LayerId New() => new(Guid.NewGuid());

    /// <summary>
    /// Tenta converter uma string para <see cref="LayerId"/>.
    /// </summary>
    public static bool TryParse(string? value, out LayerId id)
    {
        if (Guid.TryParse(value, out var parsed))
        {
            id = new LayerId(parsed);
            return true;
        }

        id = default;
        return false;
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
