using Engine.Domain.Entities;
using Engine.Domain.ValueObjects;
using Xunit;

namespace Engine.Domain.Tests.Entities;

public sealed class EffectStackTests
{
    [Fact]
    public void Add_AddsEffectsPreservingOrder()
    {
        var stack = new EffectStack();
        var first = new BlurEffect(EffectId.New(), true, 4d);
        var second = new BrightnessEffect(EffectId.New(), true, 0.2d);

        stack.Add(first);
        stack.Add(second);

        Assert.Collection(
            stack.Effects,
            effect => Assert.Equal(first.Id, effect.Id),
            effect => Assert.Equal(second.Id, effect.Id));
    }

    [Fact]
    public void Remove_RemovesEffectAndPreservesRemainingOrder()
    {
        var stack = new EffectStack();
        var first = new BlurEffect(EffectId.New(), true, 4d);
        var second = new BrightnessEffect(EffectId.New(), true, 0.1d);
        var third = new ContrastEffect(EffectId.New(), true, 0.3d);

        stack.Add(first);
        stack.Add(second);
        stack.Add(third);

        stack.Remove(second.Id);

        Assert.Collection(
            stack.Effects,
            effect => Assert.Equal(first.Id, effect.Id),
            effect => Assert.Equal(third.Id, effect.Id));
    }

    [Fact]
    public void Reorder_MovesEffectToTargetIndex()
    {
        var stack = new EffectStack();
        var first = new BlurEffect(EffectId.New(), true, 4d);
        var second = new BrightnessEffect(EffectId.New(), true, 0.1d);
        var third = new ContrastEffect(EffectId.New(), true, 0.3d);

        stack.Add(first);
        stack.Add(second);
        stack.Add(third);

        stack.Reorder(third.Id, 0);

        Assert.Collection(
            stack.Effects,
            effect => Assert.Equal(third.Id, effect.Id),
            effect => Assert.Equal(first.Id, effect.Id),
            effect => Assert.Equal(second.Id, effect.Id));
    }

    [Fact]
    public void SetEnabled_UpdatesEffectState()
    {
        var stack = new EffectStack();
        var effect = new BlurEffect(EffectId.New(), true, 8d);
        stack.Add(effect);

        stack.SetEnabled(effect.Id, false);

        Assert.False(stack.Effects.Single().IsEnabled);
    }

    [Fact]
    public void Add_WithDuplicateId_Throws()
    {
        var stack = new EffectStack();
        var effectId = EffectId.New();
        stack.Add(new BlurEffect(effectId, true, 4d));

        Assert.Throws<InvalidOperationException>(() => stack.Add(new BrightnessEffect(effectId, true, 0.2d)));
    }

    [Fact]
    public void Remove_NonExistingEffect_Throws()
    {
        var stack = new EffectStack();

        Assert.Throws<InvalidOperationException>(() => stack.Remove(EffectId.New()));
    }

    [Fact]
    public void Reorder_WithOutOfRangeIndex_Throws()
    {
        var stack = new EffectStack();
        var effect = new BlurEffect(EffectId.New(), true, 4d);
        stack.Add(effect);

        Assert.Throws<ArgumentOutOfRangeException>(() => stack.Reorder(effect.Id, 2));
    }
}
