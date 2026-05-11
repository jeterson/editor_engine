namespace Engine.Domain.ValueObjects;

/// <summary>
/// Estado de transformação 2D de uma camada, independente de backend gráfico.
/// </summary>
public readonly record struct LayerTransform
{
    public LayerTransform(double translationX, double translationY, double scaleX, double scaleY, double rotationDegrees)
    {
        EnsureFinite(translationX, nameof(translationX));
        EnsureFinite(translationY, nameof(translationY));
        EnsureFinite(scaleX, nameof(scaleX));
        EnsureFinite(scaleY, nameof(scaleY));
        EnsureFinite(rotationDegrees, nameof(rotationDegrees));

        TranslationX = translationX;
        TranslationY = translationY;
        ScaleX = scaleX;
        ScaleY = scaleY;
        RotationDegrees = rotationDegrees;
    }

    public double TranslationX { get; }

    public double TranslationY { get; }

    public double ScaleX { get; }

    public double ScaleY { get; }

    public double RotationDegrees { get; }

    public double PositionX => TranslationX;

    public double PositionY => TranslationY;

    public double Rotation => RotationDegrees;

    public static LayerTransform Identity => new(0d, 0d, 1d, 1d, 0d);

    private static void EnsureFinite(double value, string paramName)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentOutOfRangeException(paramName, "Transform values must be finite numbers.");
        }
    }
}
