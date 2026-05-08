namespace Engine.Shared;

/// <summary>
/// Represents an immutable RGBA color.
/// </summary>
/// <param name="R">Red channel.</param>
/// <param name="G">Green channel.</param>
/// <param name="B">Blue channel.</param>
/// <param name="A">Alpha channel.</param>
public readonly record struct Color(byte R, byte G, byte B, byte A)
{
    /// <summary>
    /// Creates an opaque RGB color.
    /// </summary>
    /// <param name="r">Red channel.</param>
    /// <param name="g">Green channel.</param>
    /// <param name="b">Blue channel.</param>
    /// <returns>A new opaque color.</returns>
    public static Color FromRgb(byte r, byte g, byte b) => new(r, g, b, byte.MaxValue);

    /// <summary>
    /// Creates an RGBA color.
    /// </summary>
    /// <param name="r">Red channel.</param>
    /// <param name="g">Green channel.</param>
    /// <param name="b">Blue channel.</param>
    /// <param name="a">Alpha channel.</param>
    /// <returns>A new color.</returns>
    public static Color FromRgba(byte r, byte g, byte b, byte a) => new(r, g, b, a);
}
