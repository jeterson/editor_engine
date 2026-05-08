using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

public sealed class BrightnessEffect : Effect
{
    public BrightnessEffect(EffectId id, bool isEnabled, double intensity)
        : base(id, isEnabled)
    {
        if (intensity < -1d || intensity > 1d)
        {
            throw new ArgumentOutOfRangeException(nameof(intensity), "Brightness intensity must be between -1 and 1.");
        }

        Intensity = intensity;
    }

    public double Intensity { get; }
}
