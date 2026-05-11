using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

public sealed class BrightnessEffect : Effect
{
    public BrightnessEffect(EffectId id, bool isEnabled, float intensity)
        : base(id, isEnabled)
    {

        ValidateIntensity(intensity);
        Intensity = intensity;
    }
    private void ValidateIntensity(float intensity)
    {
        if (intensity < -1d || intensity > 1d)
            throw new ArgumentOutOfRangeException(nameof(intensity), "Brightness intensity must be between -1 and 1.");
    }
    public float Intensity { get; private set; }

    public void SetIntensity(float intensity)
    {
        ValidateIntensity(intensity);
        Intensity = intensity;
    }
}
