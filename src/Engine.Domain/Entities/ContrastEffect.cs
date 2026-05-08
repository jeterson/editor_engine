using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

public sealed class ContrastEffect : Effect
{
    public ContrastEffect(EffectId id, bool isEnabled, double contrast)
        : base(id, isEnabled)
    {
        if (contrast < -1d || contrast > 1d)
        {
            throw new ArgumentOutOfRangeException(nameof(contrast), "Contrast must be between -1 and 1.");
        }

        Contrast = contrast;
    }

    public double Contrast { get; }
}
