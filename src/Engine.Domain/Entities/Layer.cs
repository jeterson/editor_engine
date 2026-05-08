using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

/// <summary>
/// Representa uma camada do documento.
/// </summary>
public sealed class Layer
{
    public Layer(LayerId id, string name, bool visibility)
    {
        if (id == default)
        {
            throw new ArgumentException("Layer id must be non-default.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Layer name must be provided.", nameof(name));
        }

        Id = id;
        Name = name;
        Visibility = visibility;
    }

    public LayerId Id { get; }

    public string Name { get; }

    public bool Visibility { get; }
}
