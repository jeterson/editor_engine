namespace Engine.Shared;

/// <summary>
/// Represents an immutable 2D size in pixels.
/// </summary>
/// <param name="Width">The horizontal size in pixels.</param>
/// <param name="Height">The vertical size in pixels.</param>
public readonly record struct Size(int Width, int Height)
{
    /// <summary>
    /// Gets an empty size (0,0).
    /// </summary>
    public static Size Empty => default;
}
