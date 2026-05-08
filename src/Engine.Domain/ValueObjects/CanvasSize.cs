using Engine.Shared;

namespace Engine.Domain.ValueObjects;

/// <summary>
/// Tamanho lógico do canvas do documento em pixels.
/// </summary>
public readonly record struct CanvasSize
{
    /// <summary>
    /// Inicializa um novo <see cref="CanvasSize"/>.
    /// </summary>
    public CanvasSize(int width, int height)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Canvas width must be greater than zero.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "Canvas height must be greater than zero.");
        }

        Width = width;
        Height = height;
    }

    /// <summary>
    /// Largura do canvas em pixels.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Altura do canvas em pixels.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Converte para <see cref="Size"/>.
    /// </summary>
    public Size ToSize() => new(Width, Height);
}
