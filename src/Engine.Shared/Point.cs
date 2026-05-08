namespace Engine.Shared;

/// <summary>
/// Represents an immutable 2D point in pixels.
/// </summary>
/// <param name="X">The horizontal coordinate in pixels.</param>
/// <param name="Y">The vertical coordinate in pixels.</param>
public readonly record struct Point(int X, int Y)
{
    /// <summary>
    /// Gets the origin point (0,0).
    /// </summary>
    public static Point Origin => default;
}
