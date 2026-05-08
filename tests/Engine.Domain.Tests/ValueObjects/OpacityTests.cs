using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Domain.Tests.ValueObjects;

public sealed class OpacityTests
{
    [Theory]
    [InlineData(0d)]
    [InlineData(0.5d)]
    [InlineData(1d)]
    public void Constructor_WithValidRange_CreatesValue(double value)
    {
        var opacity = new Opacity(value);

        Assert.Equal(value, opacity.Value);
    }

    [Theory]
    [InlineData(-0.01d)]
    [InlineData(1.01d)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Constructor_WithInvalidValue_Throws(double value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Opacity(value));
    }
}
