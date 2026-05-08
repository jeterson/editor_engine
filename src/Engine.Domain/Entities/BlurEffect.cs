using Engine.Domain.ValueObjects;

namespace Engine.Domain.Entities;

public sealed class BlurEffect : Effect
{
    public BlurEffect(EffectId id, bool isEnabled, double radius)
        : base(id, isEnabled)
    {
        if (radius < 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(radius), "Blur radius must be greater than or equal to 0.");
        }

        Radius = radius;
    }

    public double Radius { get; }
}
