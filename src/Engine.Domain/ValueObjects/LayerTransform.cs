namespace Engine.Domain.ValueObjects;

/// <summary>
/// Estado de transformação 2D de uma camada, independente de backend gráfico.
/// </summary>
public readonly record struct LayerTransform
{
    public LayerTransform(double positionX, double positionY, double scaleX, double scaleY, double rotation)
    {
        EnsureFinite(positionX, nameof(positionX));
        EnsureFinite(positionY, nameof(positionY));
        EnsureFinite(scaleX, nameof(scaleX));
        EnsureFinite(scaleY, nameof(scaleY));
        EnsureFinite(rotation, nameof(rotation));

        PositionX = positionX;
        PositionY = positionY;
        ScaleX = scaleX;
        ScaleY = scaleY;
        Rotation = rotation;
    }

    public double PositionX { get; }

    public double PositionY { get; }

    public double ScaleX { get; }

    public double ScaleY { get; }

    public double Rotation { get; }

    public static LayerTransform Identity => new(0d, 0d, 1d, 1d, 0d);

    private static void EnsureFinite(double value, string paramName)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentOutOfRangeException(paramName, "Transform values must be finite numbers.");
        }
    }
}
