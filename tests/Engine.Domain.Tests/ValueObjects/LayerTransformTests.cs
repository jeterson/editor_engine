using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Domain.Tests.ValueObjects;

public sealed class LayerTransformTests
{
    [Fact]
    public void Equality_WithSameValues_ReturnsTrue()
    {
        var left = new LayerTransform(10d, 20d, 1.2d, 0.8d, 45d);
        var right = new LayerTransform(10d, 20d, 1.2d, 0.8d, 45d);

        Assert.Equal(left, right);
    }

    [Fact]
    public void Constructor_WithInvalidFiniteValue_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LayerTransform(double.NaN, 0d, 1d, 1d, 0d));
        Assert.Throws<ArgumentOutOfRangeException>(() => new LayerTransform(0d, 0d, double.PositiveInfinity, 1d, 0d));
    }
}
