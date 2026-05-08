namespace Engine.Abstractions;

/// <summary>
/// Describes the physical characteristics and lifecycle intent of a render surface.
/// </summary>
public readonly record struct RenderSurfaceDescriptor
{
    public RenderSurfaceDescriptor(int width, int height, PixelFormat pixelFormat, bool isHighPrecision, RenderResourceLifetime lifetime)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be greater than zero.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be greater than zero.");
        }

        Width = width;
        Height = height;
        PixelFormat = pixelFormat;
        IsHighPrecision = isHighPrecision;
        Lifetime = lifetime;
    }

    public int Width { get; }

    public int Height { get; }

    public PixelFormat PixelFormat { get; }

    public bool IsHighPrecision { get; }

    public RenderResourceLifetime Lifetime { get; }
}
