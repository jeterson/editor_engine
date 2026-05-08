namespace Engine.Shared;

/// <summary>
/// Represents an immutable axis-aligned rectangle in pixels.
/// </summary>
/// <param name="Position">The top-left position.</param>
/// <param name="Size">The rectangle size.</param>
public readonly record struct Rect(Point Position, Size Size)
{
    /// <summary>
    /// Gets the horizontal size in pixels.
    /// </summary>
    public int Width => Size.Width;

    /// <summary>
    /// Gets the vertical size in pixels.
    /// </summary>
    public int Height => Size.Height;

    /// <summary>
    /// Gets the left edge coordinate.
    /// </summary>
    public int Left => Position.X;

    /// <summary>
    /// Gets the top edge coordinate.
    /// </summary>
    public int Top => Position.Y;

    /// <summary>
    /// Gets the right edge coordinate.
    /// </summary>
    public int Right => Position.X + Size.Width;

    /// <summary>
    /// Gets the bottom edge coordinate.
    /// </summary>
    public int Bottom => Position.Y + Size.Height;

    /// <summary>
    /// Determines whether this rectangle contains the specified point.
    /// </summary>
    /// <param name="point">The point to evaluate.</param>
    /// <returns><see langword="true"/> when the point is inside the rectangle; otherwise <see langword="false"/>.</returns>
    public bool Contains(Point point)
        => point.X >= Left && point.X < Right && point.Y >= Top && point.Y < Bottom;

    /// <summary>
    /// Determines whether this rectangle intersects another rectangle.
    /// </summary>
    /// <param name="other">The other rectangle.</param>
    /// <returns><see langword="true"/> when the rectangles intersect; otherwise <see langword="false"/>.</returns>
    public bool Intersects(Rect other)
        => Left < other.Right && Right > other.Left && Top < other.Bottom && Bottom > other.Top;
}
