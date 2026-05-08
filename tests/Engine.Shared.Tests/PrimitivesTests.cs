namespace Engine.Shared.Tests;

public sealed class PrimitivesTests
{
    [Fact]
    public void Size_And_Point_Equality_Works()
    {
        Assert.Equal(new Size(10, 20), new Size(10, 20));
        Assert.NotEqual(new Point(1, 2), new Point(2, 1));
    }

    [Fact]
    public void Rect_Exposes_Size_And_Position()
    {
        var rect = new Rect(new Point(3, 4), new Size(10, 20));

        Assert.Equal(10, rect.Width);
        Assert.Equal(20, rect.Height);
        Assert.Equal(new Point(3, 4), rect.Position);
    }

    [Fact]
    public void Rect_Contains_Uses_HalfOpen_Bounds()
    {
        var rect = new Rect(new Point(0, 0), new Size(10, 10));

        Assert.True(rect.Contains(new Point(0, 0)));
        Assert.True(rect.Contains(new Point(9, 9)));
        Assert.False(rect.Contains(new Point(10, 9)));
        Assert.False(rect.Contains(new Point(9, 10)));
    }

    [Fact]
    public void Rect_Intersects_Works()
    {
        var a = new Rect(new Point(0, 0), new Size(10, 10));
        var b = new Rect(new Point(5, 5), new Size(2, 2));
        var c = new Rect(new Point(10, 10), new Size(3, 3));

        Assert.True(a.Intersects(b));
        Assert.False(a.Intersects(c));
    }

    [Fact]
    public void Color_Supports_Rgba_And_Equality()
    {
        var color = Color.FromRgb(1, 2, 3);

        Assert.Equal(new Color(1, 2, 3, 255), color);
        Assert.Equal(Color.FromRgba(1, 2, 3, 4), new Color(1, 2, 3, 4));
    }

    [Fact]
    public void PixelFormat_Defines_Supported_Formats()
    {
        Assert.Equal(0, (int)PixelFormat.Rgba8);
        Assert.Equal(1, (int)PixelFormat.Bgra8);
        Assert.Equal(2, (int)PixelFormat.Rgba16Float);
    }
}
