namespace Engine.Domain.ValueObjects;

/// <summary>
/// Representa a opacidade de uma camada no intervalo [0, 1].
/// </summary>
public readonly record struct Opacity
{
    public Opacity(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Opacity must be a finite number.");
        }

        if (value < 0d || value > 1d)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Opacity must be between 0 and 1.");
        }

        Value = value;
    }

    public double Value { get; }

    public static Opacity Opaque => new(1d);

    public static Opacity Transparent => new(0d);

    public override string ToString() => Value.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
}
