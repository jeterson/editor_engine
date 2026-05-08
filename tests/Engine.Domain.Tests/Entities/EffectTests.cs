using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Domain.Tests.Entities;

public sealed class EffectTests
{
    [Fact]
    public void BlurEffect_Creation_StoresTypedParameters()
    {
        var effect = new BlurEffect(EffectId.New(), true, 12.5d);

        Assert.Equal(12.5d, effect.Radius);
        Assert.True(effect.IsEnabled);
    }

    [Fact]
    public void BrightnessEffect_Creation_StoresTypedParameters()
    {
        var effect = new BrightnessEffect(EffectId.New(), false, -0.2d);

        Assert.Equal(-0.2d, effect.Intensity);
        Assert.False(effect.IsEnabled);
    }

    [Fact]
    public void ContrastEffect_Creation_StoresTypedParameters()
    {
        var effect = new ContrastEffect(EffectId.New(), true, 0.8d);

        Assert.Equal(0.8d, effect.Contrast);
    }

    [Fact]
    public void SetEnabled_UpdatesEffectState()
    {
        var effect = new ContrastEffect(EffectId.New(), true, 0.1d);

        effect.SetEnabled(false);

        Assert.False(effect.IsEnabled);
    }

    [Fact]
    public void Effects_WithSameData_AreNotEqualBecauseIdentityDiffers()
    {
        var first = new BlurEffect(EffectId.New(), true, 5d);
        var second = new BlurEffect(EffectId.New(), true, 5d);

        Assert.NotEqual(first.Id, second.Id);
    }

    [Theory]
    [InlineData(-0.1d)]
    public void BlurEffect_WithInvalidRadius_Throws(double radius)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BlurEffect(EffectId.New(), true, radius));
    }

    [Theory]
    [InlineData(-1.1d)]
    [InlineData(1.1d)]
    public void BrightnessEffect_WithInvalidIntensity_Throws(double intensity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BrightnessEffect(EffectId.New(), true, intensity));
    }

    [Theory]
    [InlineData(-1.1d)]
    [InlineData(1.1d)]
    public void ContrastEffect_WithInvalidContrast_Throws(double contrast)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ContrastEffect(EffectId.New(), true, contrast));
    }
}
