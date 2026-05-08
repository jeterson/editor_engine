using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

/// <summary>
/// Representa uma camada do documento.
/// </summary>
public sealed class Layer
{
    public Layer(
        LayerId id,
        string name,
        bool visibility,
        LayerTransform transform,
        Opacity opacity,
        BlendMode blendMode)
    {
        if (id == default)
        {
            throw new ArgumentException("Layer id must be non-default.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Layer name must be provided.", nameof(name));
        }

        ValidateBlendMode(blendMode);

        Id = id;
        Name = name;
        Visibility = visibility;
        Transform = transform;
        Opacity = opacity;
        BlendMode = blendMode;
    }

    public LayerId Id { get; }

    public string Name { get; private set; }

    public bool Visibility { get; private set; }

    public LayerTransform Transform { get; private set; }

    public Opacity Opacity { get; private set; }

    public BlendMode BlendMode { get; private set; }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Layer name must be provided.", nameof(name));
        }

        Name = name;
    }

    public void SetVisibility(bool visibility) => Visibility = visibility;

    public void SetTransform(LayerTransform transform) => Transform = transform;

    public void SetOpacity(Opacity opacity) => Opacity = opacity;

    public void SetBlendMode(BlendMode blendMode)
    {
        ValidateBlendMode(blendMode);
        BlendMode = blendMode;
    }

    private static void ValidateBlendMode(BlendMode blendMode)
    {
        if (!Enum.IsDefined(blendMode))
        {
            throw new ArgumentOutOfRangeException(nameof(blendMode), "Unsupported blend mode.");
        }
    }
}
